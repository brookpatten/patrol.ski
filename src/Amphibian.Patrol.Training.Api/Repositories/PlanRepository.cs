using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using AutoMapper;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class PlanRepository: IPlanRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public PlanRepository(IDbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public Task<IEnumerable<Plan>> GetPlansForPatrol(int patrolId)
        {
            return _connection.QueryAsync<Plan>(@"select Id,PatrolId,Name from Plans where patrolId=@patrolId", new { patrolId });
        }

        public async Task InsertPlan(Plan plan)
        {
            await _connection.InsertAsync(plan).ConfigureAwait(false);
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
                    ps.sectionid=s.id and ps.planid=@planId", new { planId });
        }

        public async Task<IEnumerable<SectionSkill>> GetSectionSkills(int sectionId)
        {
            return await _connection.QueryAsync<SectionSkill>(
                @"select 
                    id
                    ,sectionid
                    ,skillid
                    ,rowindex
                from sectionskills 
                where sectionid=@sectionId",new { sectionId });
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
                },splitOn:"id",param:new { planId });
        }

        public async Task<IEnumerable<Skill>> GetSkills(int patrolId)
        {
            return await _connection.QueryAsync<Skill>(
                @"select 
                    id
                    ,name
                    ,description
                    ,patrolid
                from skills 
                where patrolId=@patrolId", new { patrolId });
        }

        public async Task<IEnumerable<SectionLevel>> GetSectionLevels(int sectionId)
        {
            return await _connection.QueryAsync<SectionLevel>(
                @"select 
                    id
                    ,sectionid
                    ,levelid
                    ,columnindex
                from sectionlevels 
                where sectionid=@sectionId", new { sectionId });
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
                },splitOn:"id",param:new { planId });
        }

        public async Task<IEnumerable<Level>> GetLevels(int patrolId)
        {
            return await _connection.QueryAsync<Level>(
                @"select 
                    id
                    ,name
                    ,description
                    ,patrolid
                from levels 
                where patrolId=@patrolId", new { patrolId });
        }
    }
}
