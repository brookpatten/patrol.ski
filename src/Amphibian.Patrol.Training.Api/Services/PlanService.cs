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
        private ILogger<PlanService> _logger;
        private IMapper _mapper;
        public PlanService(IPlanRepository planRepository, ILogger<PlanService> logger, IMapper mapper)
        {
            _planRepository = planRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PlanDto> GetPlan(int id)
        {
            var plan = await _planRepository.GetPlan(id);
            var sections = await _planRepository.GetSectionsForPlan(plan.Id);
            var sectionSkills = await _planRepository.GetSectionSkillsForPlan(plan.Id);
            var sectionLevels = await _planRepository.GetSectionLevelsForPlan(plan.Id);

            var planDto = _mapper.Map<Plan, PlanDto>(plan);
            planDto.Sections = _mapper.Map<IEnumerable<Section>, IEnumerable<SectionDto>>(sections);

            foreach (var section in planDto.Sections)
            {
                section.Skills = _mapper.Map<IEnumerable<SectionSkill>, IEnumerable<SectionSkillDto>>(sectionSkills.Where(x => x.SectionId == section.Id));
                section.Levels = _mapper.Map<IEnumerable<SectionLevel>, IEnumerable<SectionLevelDto>>(sectionLevels.Where(x => x.SectionId == section.Id));
            }

            return planDto;
        }
    }
}
