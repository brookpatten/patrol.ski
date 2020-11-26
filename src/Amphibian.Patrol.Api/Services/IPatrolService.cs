using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IPatrolService
    {
        Task<Role?> GetUserRoleInPatrol(int userId, int patrolId);
        Task DeleteGroup(int groupId);
    }
}
