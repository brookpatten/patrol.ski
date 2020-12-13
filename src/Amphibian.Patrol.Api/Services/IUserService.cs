using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IUserService
    {
        Task<User> AddUserToPatrol(int patrolId, Role? role, string firstName, string lastName, string email);
        Task AddUserToGroup(int userId, int groupId);
        Task RemoveUserFromGroup(int userId, int groupId);
        Task<IEnumerable<PatrolUserDto>> GetPatrolUsers(int patrolId);
        Task<PatrolUserDto> GetPatrolUser(int patrolId, int userId);
        Task UpdatePatrolUser(PatrolUserDto dto);
        Task RemovePersonalInformation(int userId);
    }
}
