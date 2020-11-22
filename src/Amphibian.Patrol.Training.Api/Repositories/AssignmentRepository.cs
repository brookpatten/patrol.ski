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
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IDbConnection _connection;

        public AssignmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<IEnumerable<Assignment>> GetAssignmentsForUser(int userId)
        {
            return _connection.QueryAsync<Assignment>(@"select Id,planid,userid,assignedat,dueat from assignments where userid=@userId", new { userId });
        }

        public Task<IEnumerable<Assignment>> GetAssignmentsForPlan(int planId)
        {
            return _connection.QueryAsync<Assignment>(@"select Id,planid,userid,assignedat,dueat from assignments where planId=@planId", new { planId });
        }

        public Task<Assignment> GetAssignment(int assignmentId)
        {
            return _connection.GetAsync<Assignment>(assignmentId);
        }
    }
}
