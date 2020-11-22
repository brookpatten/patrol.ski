using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class AssignmentService : IAssignmentService
    {
        private IAssignmentRepository _assignmentRepository;
        private ILogger<AssignmentService> _logger;
        private IMapper _mapper;
        private IPlanService _planService;
        public AssignmentService(IAssignmentRepository assignmentRepository,IPlanService planService, ILogger<AssignmentService> logger, IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _logger = logger;
            _mapper = mapper;
            _planService = planService;
        }

        public async Task<AssignmentDto> GetAssignment(int id)
        {
            var assignment = await _assignmentRepository.GetAssignment(id);
            var signatures = await _assignmentRepository.GetSignaturesWithUsersForAssignment(id);

            var assignmentDto = _mapper.Map<Assignment, AssignmentDto>(assignment);
            assignmentDto.Signatures = signatures;

            return assignmentDto;
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
    }
}
