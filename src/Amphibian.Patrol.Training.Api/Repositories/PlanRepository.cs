using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

using Amphibian.Patrol.Training.Api.Models;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class PlanRepository: IPlanRepository
    {
        private readonly IDbConnection _connection;

        public PlanRepository(IDbConnection connection)
        {
            _connection = connection;
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
                    ,[order] 
                from sectionskills 
                where sectionid=@sectionId",new { sectionId });
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
                    ,[order] 
                from sectionlevels 
                where sectionid=@sectionId", new { sectionId });
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
