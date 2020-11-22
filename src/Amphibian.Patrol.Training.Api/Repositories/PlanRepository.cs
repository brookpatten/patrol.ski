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

    }
}
