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
    public class PlanService : IPlanService
    {
        private IPlanRepository _planRepository;
        private IGroupRepository _groupRepository;
        private ILogger<PlanService> _logger;
        private IMapper _mapper;
        public PlanService(IPlanRepository planRepository, ILogger<PlanService> logger, IMapper mapper,IGroupRepository groupRepository)
        {
            _planRepository = planRepository;
            _logger = logger;
            _mapper = mapper;
            _groupRepository = groupRepository;
        }

        public async Task<PlanDto> GetPlan(int id, int? currentUserId=null)
        {
            var plan = await _planRepository.GetPlan(id);
            var sections = await _planRepository.GetSectionsForPlan(plan.Id);
            var sectionSkills = await _planRepository.GetSectionSkillsForPlan(plan.Id);
            var sectionLevels = await _planRepository.GetSectionLevelsForPlan(plan.Id);

            var sectionIdsCanSign = new List<int>();
            if (currentUserId.HasValue)
            {
                sectionIdsCanSign = (await _groupRepository.GetSectionIdsInPlanThatUserCanSign(currentUserId.Value, id)).ToList();
            }

            var planDto = _mapper.Map<Plan, PlanDto>(plan);
            planDto.Sections = _mapper.Map<IEnumerable<Section>, IEnumerable<SectionDto>>(sections);

            foreach (var section in planDto.Sections)
            {
                section.CurrentUserCanSign = sectionIdsCanSign.Contains(section.Id);
                section.Skills = sectionSkills.Where(x => x.SectionId == section.Id);
                section.Levels = sectionLevels.Where(x => x.SectionId == section.Id);
            }

            return planDto;
        }
    }
}
