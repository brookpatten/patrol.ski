using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IPatrolService
    {
        Task<Role?> GetUserRoleInPatrol(int userId, int patrolId);
    }
}
