using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dommel;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using AutoMapper;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class GroupRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public GroupRepository(IDbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetUsersInGroup(int groupId)
        {
            return await _connection.QueryAsync<User>(@"");
        }

        public async Task<IEnumerable<Group>> GetGroupsForUser(int patrolId, int userId)
        {
            return await _connection.QueryAsync<Group>(@"select distinct g.* from groupusers gu inner join groups g on g.id=gu.groupid and gu.userid=@userId where g.patrolid=@patrolId", new { patrolId, userId });
        }
    }
}
