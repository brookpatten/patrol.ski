﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Serilog.Configuration;
using Microsoft.CodeAnalysis.CSharp;

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

        /// <summary>
        /// create a training plan in the designated patrol, clone the designated plan contents if specified
        /// specified plan can be from any patrol, relevant skills/levels will be copied to target patrol as needed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="patrolId"></param>
        /// <param name="copyFromPlanId"></param>
        /// <returns></returns>
        public async Task<Plan> CreatePlan(string name,int patrolId, int? copyFromPlanId)
        {
            var newPlan = new Plan()
            {
                Name = name,
                PatrolId = patrolId
            };
            await _planRepository.InsertPlan(newPlan);

            if(copyFromPlanId.HasValue)
            {
                var sections = await _planRepository.GetSectionsForPlan(copyFromPlanId.Value);
                var sectionSkills = await _planRepository.GetSectionSkillsForPlan(copyFromPlanId.Value);
                var sectionLevels = await _planRepository.GetSectionLevelsForPlan(copyFromPlanId.Value);

                var patrolLevels = (await _planRepository.GetLevels(patrolId)).ToList();
                var patrolSkills = (await _planRepository.GetSkills(patrolId)).ToList();

                foreach(var section in sections)
                {
                    var newSection = new Section()
                    {
                        Name = section.Name,
                        PatrolId = section.PatrolId
                    };
                    await _planRepository.InsertSection(newSection);

                    var planSection = new PlanSection()
                    {
                        PlanId = newPlan.Id,
                        SectionId = newSection.Id
                    };
                    await _planRepository.InsertPlanSection(planSection);

                    var levels = sectionLevels.Where(x => x.SectionId == section.Id);
                    foreach (var sectionLevel in levels)
                    {
                        int levelId;
                        if(sectionLevel.Level.PatrolId==patrolId)
                        {
                            levelId = sectionLevel.Level.Id;
                        }
                        else
                        {
                            if(patrolLevels.Any(x=>x.Name==sectionLevel.Level.Name))
                            {
                                levelId = patrolLevels.First(x => x.Name == sectionLevel.Level.Name).Id;
                            }
                            else
                            {
                                var newLevel = new Level()
                                {
                                    Name = sectionLevel.Level.Name,
                                    Description = sectionLevel.Level.Description,
                                    PatrolId = patrolId
                                };
                                await _planRepository.InsertLevel(newLevel);
                                patrolLevels.Add(newLevel);
                                levelId = newLevel.Id;
                            }
                        }

                        var newSectionLevel = new SectionLevel()
                        {
                            ColumnIndex = sectionLevel.ColumnIndex,
                            LevelId = levelId,
                            SectionId = newSection.Id
                        };
                        await _planRepository.InsertSectionLevel(newSectionLevel);
                    }

                    var skills = sectionSkills.Where(x => x.SectionId == section.Id);
                    foreach (var sectionSkill in skills)
                    {
                        int skillId;
                        if (sectionSkill.Skill.PatrolId == patrolId)
                        {
                            skillId = sectionSkill.Skill.Id;
                        }
                        else
                        {
                            if (patrolSkills.Any(x => x.Name == sectionSkill.Skill.Name))
                            {
                                skillId = patrolSkills.First(x => x.Name == sectionSkill.Skill.Name).Id;
                            }
                            else
                            {
                                var newSkill = new Skill()
                                {
                                    Name = sectionSkill.Skill.Name,
                                    Description = sectionSkill.Skill.Description,
                                    PatrolId = patrolId
                                };
                                await _planRepository.InsertSkill(newSkill);
                                patrolSkills.Add(newSkill);
                                skillId = newSkill.Id;
                            }
                        }

                        var newSectionSkill = new SectionSkill()
                        {
                            RowIndex = sectionSkill.RowIndex,
                            SkillId = skillId,
                            SectionId = newSection.Id
                        };
                        await _planRepository.InsertSectionSkill(newSectionSkill);
                    }
                }
            }

            return newPlan;
        }
        public async Task<bool> IsPlanFormatValid(PlanDto plan)
        {
            if(string.IsNullOrEmpty(plan.Name))
            {
                return false;
            }

            if(plan.Sections!=null)
            {
                foreach(var section in plan.Sections)
                {
                    //ensure levels are contiguous
                    if(section.Levels!=null && section.Levels.Any())
                    {
                        var min = section.Levels.Min(x => x.ColumnIndex);
                        var max = section.Levels.Max(x => x.ColumnIndex);
                        for(var i=min;i<=max;i++)
                        {
                            var sectionLevel = section.Levels.SingleOrDefault(x => x.ColumnIndex == i);
                            if(sectionLevel==null || sectionLevel.Level == null || sectionLevel.Level.Id == default(int))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                    //ensure skills are contiguous
                    if (section.Skills != null && section.Skills.Any())
                    {
                        var min = section.Skills.Min(x => x.RowIndex);
                        var max = section.Skills.Max(x => x.RowIndex);
                        for (var i = min; i <= max; i++)
                        {
                            var sectionSkill = section.Skills.SingleOrDefault(x => x.RowIndex == i);
                            if (sectionSkill == null || sectionSkill.Skill == null || sectionSkill.Skill.Id == default(int))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        public async Task UpdatePlan(PlanDto dto)
        {
            var plan = await _planRepository.GetPlan(dto.Id);
            plan.Name = dto.Name;
            await _planRepository.UpdatePlan(plan);

            var allLevels = (await _planRepository.GetLevels(dto.PatrolId)).ToList();
            var allSkills = (await _planRepository.GetSkills(dto.PatrolId)).ToList();

            //update existing sections
            var sections = await _planRepository.GetSectionsForPlan(dto.Id);
            foreach(var section in sections)
            {
                var newSection = dto.Sections.SingleOrDefault(x => x.Id == section.Id);
                
                if(newSection==null)
                {
                    //remove
                    
                }
                else
                {
                    //sync
                }
            }

            //create new sections
            foreach(var newSection in dto.Sections.Where(x=>x.Id==default(int)))
            {
                //save new sections
            }
        }
    }
}
