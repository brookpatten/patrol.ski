using Amphibian.Patrol.Training.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IPatrolRepository
    {
        Task DeletePatrolUser(int patrolId, int userId);
        Task<Models.Patrol> GetPatrol(int id);
        Task<IEnumerable<Models.Patrol>> GetPatrolsForUser(int userId);
        Task<IEnumerable<PatrolUser>> GetPatrolUsersForPatrol(int patrolId);
        Task<IEnumerable<PatrolUser>> GetPatrolUsersForUser(int userId);
        Task<PatrolUser> GetPatrolUser(int userId, int patrolId);
        Task<IEnumerable<User>> GetUsersForPatrol(int patrolId);
        Task InsertPatrol(Models.Patrol patrol);
        Task InsertPatrolUser(PatrolUser patrolUser);
        Task UpdatePatrol(Models.Patrol patrol);
        Task<PatrolUser> GetPatrolUser(int id);
    }
}