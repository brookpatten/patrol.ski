using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Api.Services
{
    public class AssignmentService : IAssignmentService
    {
        private IAssignmentRepository _assignmentRepository;
        private ILogger<AssignmentService> _logger;
        private IMapper _mapper;
        private IPlanService _planService;
        private IPlanRepository _planRepository;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        private ISystemClock _clock;
        public AssignmentService(IAssignmentRepository assignmentRepository,IPlanService planService, ILogger<AssignmentService> logger, 
            IPlanRepository planRepository, IPatrolRepository patrolRepository, IGroupRepository groupRepository,IMapper mapper, ISystemClock clock)
        {
            _assignmentRepository = assignmentRepository;
            _logger = logger;
            _mapper = mapper;
            _planService = planService;
            _planRepository = planRepository;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
            _clock = clock;
        }

        public async Task<AssignmentDto> GetAssignment(int id)
        {
            var assignment = await _assignmentRepository.GetAssignment(id);
            var signatures = await _assignmentRepository.GetSignaturesWithUsersForAssignment(id);

            var assignmentDto = _mapper.Map<Assignment, AssignmentDto>(assignment);
            assignmentDto.Signatures = signatures;

            return assignmentDto;
        }

        public async Task<bool> AllowCreateSignatures(int assignmentId, int byUserId, IList<NewSignatureDto> newSignatures)
        {
            //ensure the specified assignment has a plan under a patrol the user has access to
            var assignment = await _assignmentRepository.GetAssignment(assignmentId);
            var plan = await _planRepository.GetPlan(assignment.PlanId);
            var patrols = await _patrolRepository.GetPatrolsForUser(byUserId);
            if(!patrols.Any(x=>x.Id==plan.PatrolId))
            {
                return false;
            }
            
            var sections = await _planRepository.GetSectionsForPlan(plan.Id);
            var sectionLevels = await _planRepository.GetSectionLevelsForPlan(plan.Id);
            var sectionSkills = await _planRepository.GetSectionSkillsForPlan(plan.Id);

            var sectionsUserCanSign = await _groupRepository.GetSectionIdsInPlanThatUserCanSign(byUserId, plan.Id);

            foreach(var sig in newSignatures)
            {
                var sectionLevel = sectionLevels.SingleOrDefault(x => x.Id == sig.SectionLevelId);
                var sectionSkill = sectionSkills.SingleOrDefault(x => x.Id == sig.SectionSkillId);

                if(sectionLevel==null || sectionSkill==null || sectionLevel.SectionId != sectionSkill.SectionId)
                {
                    //the specified leve/skill ids are not in the plan
                    return false;
                }

                var section = sections.Single(x => x.Id == sectionLevel.SectionId);
                if(!sectionsUserCanSign.Any(x=>x==section.Id))
                {
                    //the user is not allowed to sign the section per group rules
                    return false;
                }
            }

            return true;
        }

        public async Task CreateSignatures(int assignmentId, int byUserId, IList<NewSignatureDto> newSignatures)
        {
            var existingSignatures = await _assignmentRepository.GetSignaturesForAssignment(assignmentId);
            var now = DateTime.UtcNow;

            //remove any "new" signatures that were previously signed by someone else
            var uniqueSignatures = newSignatures.Where(x => !existingSignatures.Any(y => y.SectionLevelId == x.SectionLevelId && y.SectionSkillId == x.SectionSkillId));

            var allSignatures = new List<Signature>();
            allSignatures.AddRange(existingSignatures);

            foreach (var sig in uniqueSignatures)
            {
                var signature = new Signature()
                {
                    AssignmentId = assignmentId,
                    SectionLevelId = sig.SectionLevelId,
                    SectionSkillId = sig.SectionSkillId,
                    SignedByUserId = byUserId,
                    SignedAt = now
                };
                await _assignmentRepository.InsertSignature(signature);
                allSignatures.Add(signature);
            }

            //check to see if it's time to complete the assignment
            var assignment = await _assignmentRepository.GetAssignment(assignmentId);
            if (!assignment.CompletedAt.HasValue)
            {
                if (await IsComplete(assignment, allSignatures))
                {
                    await CompleteAssignment(assignment, now);
                }
            }
        }

        private async Task<bool> IsComplete(Assignment assignment,IList<Signature> signatures)
        {
            var plan = await _planService.GetPlan(assignment.PlanId);

            foreach(var section in plan.Sections)
            {
                foreach(var skill in section.Skills)
                {
                    foreach(var level in section.Levels)
                    {
                        var sig = signatures.Where(x => x.SectionLevelId == level.Id
                            && x.SectionSkillId == skill.Id
                            && x.AssignmentId == assignment.Id).FirstOrDefault();

                        if(sig==null)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private async Task CompleteAssignment(Assignment assignment, DateTime at)
        {
            assignment.CompletedAt = at;
            await _assignmentRepository.UpdateAssignment(assignment);
        }

        public async Task CreateAssignments(int planId,IList<int> toUserIds,DateTime? dueAt)
        {
            var now = _clock.UtcNow.UtcDateTime;
            foreach(var userId in toUserIds)
            {
                var assignment = new Assignment()
                {
                    AssignedAt = now,
                    DueAt = dueAt,
                    PlanId = planId,
                    UserId = userId
                };
                await _assignmentRepository.InsertAssignment(assignment);
            }
        }
    }
}
