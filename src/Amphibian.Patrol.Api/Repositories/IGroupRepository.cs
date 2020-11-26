using Amphibian.Patrol.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetGroupsForUser(int patrolId, int userId);
        Task<IEnumerable<GroupUser>> GetGroupUsersForUser(int patrolId, int userId);
        Task<IEnumerable<GroupUser>> GetGroupsForUsers(int patrolId, IList<int> userIds);
        Task<IEnumerable<GroupUser>> GetGroupUsersForGroup(int groupId);
        Task<IEnumerable<User>> GetUsersInGroup(int groupId);
        Task<IEnumerable<Group>> GetGroupsForSection(int sectionId);
        Task<IEnumerable<int>> GetSectionIdsInPlanThatUserCanSign(int userId, int planId);
        Task<Group> GetGroup(int id);
        Task<GroupUser> InsertGroupUser(GroupUser groupUser);
        Task DeleteGroupUser(GroupUser groupUser);
        Task DeleteGroup(Group group);
        Task<GroupUser> GetGroupUser(int userId, int groupId);
        Task<IEnumerable<Group>> GetGroupsForPatrol(int patrolId);
        Task<Group> InsertGroup(Group group);
        Task UpdateGroup(Group group);
    }
}