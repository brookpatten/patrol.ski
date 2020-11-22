using Amphibian.Patrol.Training.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetGroupsForUser(int patrolId, int userId);
        Task<IEnumerable<User>> GetUsersInGroup(int groupId);
        Task<IEnumerable<Group>> GetGroupsForSection(int sectionId);
        Task<IEnumerable<int>> GetSectionIdsInPlanThatUserCanSign(int userId, int planId);
    }
}