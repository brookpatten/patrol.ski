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

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDbConnection _connection;
        
        public GroupRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<User>> GetUsersInGroup(int groupId)
        {
            return await _connection.QueryAsync<User>(
                @"select distinct u.* 
                from groupusers gu 
                inner join users u on u.id=gu.userid
                where gu.groupid=@groupId", new { groupId });
        }

        public async Task<IEnumerable<Group>> GetGroupsForUser(int patrolId, int userId)
        {
            return await _connection.QueryAsync<Group>(
                @"select distinct g.* 
                from groupusers gu 
                inner join groups g on g.id=gu.groupid 
                    and gu.userid=@userId 
                where g.patrolid=@patrolId", new { patrolId, userId });
        }

        public async Task<IEnumerable<Group>> GetGroupsForSection(int sectionId)
        {
            return await _connection.QueryAsync<Group>(
                @"select distinct g.* 
                from sectiongroups gu 
                inner join groups g on g.id=gu.groupid
                where gu.sectionid=@sectionId", new { sectionId });
        }

        public async Task<IEnumerable<int>> GetSectionIdsInPlanThatUserCanSign(int userId, int planId)
        {
            return await _connection.QueryAsync<int>(
                @"select ps.sectionid
                from groupusers gu 
                inner join sectiongroups sg on sg.groupid=gu.groupid
                inner join plansections ps on ps.sectionid=sg.sectionid and ps.planid=@planId
                where gu.userid=@userId", new { userId,planId });
        }
    }
}
