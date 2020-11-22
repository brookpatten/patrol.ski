using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class PatrolService
    {
        private IPatrolRepository _patrolRepository;
        public PatrolService(IPatrolRepository patrolRepository)
        {
            _patrolRepository = patrolRepository;
        }
        public async Task<Role?> GetUserRoleInPatrol(int userId,int patrolId)
        {
            var patrolUser = await _patrolRepository.GetPatrolUser(userId, patrolId);

            if(patrolUser!=null)
            {
                return patrolUser.Role;
            }
            else
            {
                return null;
            }
        }
    }
}
