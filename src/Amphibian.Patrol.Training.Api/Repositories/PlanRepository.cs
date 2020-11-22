using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dommel;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using AutoMapper;
using System.Data.Common;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class PlanRepository: IPlanRepository
    {
        private readonly DbConnection _connection;
        private readonly IMapper _mapper;

        public PlanRepository(DbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Plan>> GetPlansForPatrol(int patrolId)
        {
            return await _connection.SelectAsync<Plan>(x => x.PatrolId == patrolId).ConfigureAwait(false);
        }

        public async Task InsertPlan(Plan plan)
        {
            plan.Id = (int)await _connection.InsertAsync(plan).ConfigureAwait(false);
        }
        public async Task UpdatePlan(Plan plan)
        {
            await _connection.UpdateAsync(plan).ConfigureAwait(false);
        }
        public async Task<Plan> GetPlan(int id)
        {
            return await _connection.GetAsync<Plan>(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Section>> GetSectionsForPlan(int planId)
        {
            return await _connection.QueryAsync<Section>(
                @"select 
                    s.id
                    ,s.name
                    ,s.patrolid 
                from sections s 
                inner join plansections ps on 
                    ps.sectionid=s.id and ps.planid=@planId", new { planId }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SectionSkill>> GetSectionSkills(int sectionId)
        {
            return await _connection.SelectAsync<SectionSkill>(x => x.SectionId == sectionId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SectionSkillDto>> GetSectionSkillsForPlan(int planId)
        {
            return await _connection.QueryAsync<SectionSkill,Skill,SectionSkillDto>(
                @"select 
                    ss.id
                    ,ss.sectionid
                    ,ss.skillid
                    ,ss.rowindex
                    ,sks.id
                    ,sks.patrolid
                    ,sks.name
                    ,sks.description
                from sectionskills ss
                inner join sections s on s.id=ss.sectionid
                inner join skills sks on sks.id=ss.skillid
                inner join plansections ps on ps.sectionid=s.id and ps.planid=@planId",(ss,s)=>
                {
                    var dto = _mapper.Map<SectionSkill, SectionSkillDto>(ss);
                    dto.Skill = s;
                    return dto;
                },splitOn:"id",param:new { planId }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Skill>> GetSkills(int patrolId)
        {
            return await _connection.SelectAsync<Skill>(x => x.PatrolId == patrolId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SectionLevel>> GetSectionLevels(int sectionId)
        {
            return await _connection.SelectAsync<SectionLevel>(x => x.SectionId == sectionId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SectionLevelDto>> GetSectionLevelsForPlan(int planId)
        {
            return await _connection.QueryAsync<SectionLevel,Level,SectionLevelDto>(
                @"select 
                    ss.id
                    ,ss.sectionid
                    ,ss.levelid
                    ,ss.columnindex
                    ,l.id
                    ,l.patrolid
                    ,l.name
                    ,l.description
                from sectionlevels ss
                inner join sections s on s.id=ss.sectionid 
                inner join levels l on l.id=ss.levelid
                inner join plansections ps on ps.sectionid=s.id and ps.planid=@planId",(sl,l)=>
                {
                    var dto = _mapper.Map<SectionLevel, SectionLevelDto>(sl);
                    dto.Level = l;
                    return dto;
                },splitOn:"id",param:new { planId }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Level>> GetLevels(int patrolId)
        {
            return await _connection.SelectAsync<Level>(x => x.PatrolId == patrolId).ConfigureAwait(false);
        }

        public async Task InsertSection(Section section)
        {
            section.Id = (int)await _connection.InsertAsync(section).ConfigureAwait(false);
        }
        public async Task InsertPlanSection(PlanSection planSection)
        {
            planSection.Id = (int)await _connection.InsertAsync(planSection).ConfigureAwait(false);
        }
        public async Task InsertSectionLevel(SectionLevel sectionLevel)
        {
            sectionLevel.Id = (int)await _connection.InsertAsync(sectionLevel).ConfigureAwait(false);
        }
        public async Task InsertSectionSkill(SectionSkill sectionSkill)
        {
            sectionSkill.Id = (int)await _connection.InsertAsync(sectionSkill).ConfigureAwait(false);
        }
        public async Task InsertLevel(Level level)
        {
            level.Id = (int)await _connection.InsertAsync(level).ConfigureAwait(false);
        }
        public async Task InsertSkill(Skill skill)
        {
            skill.Id = (int)await _connection.InsertAsync(skill).ConfigureAwait(false);
        }
    }
}
