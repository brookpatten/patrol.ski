using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class PatrolService:IPatrolService
    {
        private IPatrolRepository _patrolRepository;
        private IPlanRepository _planRepository;
        private IGroupRepository _groupRepository;
        public PatrolService(IPatrolRepository patrolRepository, IPlanRepository planRepository, IGroupRepository groupRepository)
        {
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _groupRepository = groupRepository;
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

        public async Task DeleteGroup(int groupId)
        {
            var group = await _groupRepository.GetGroup(groupId);
            var members = await _groupRepository.GetGroupUsersForGroup(groupId);
            var sectionGroups = await _planRepository.GetSectionGroupsForGroup(groupId);

            foreach(var sectionGroup in sectionGroups)
            {
                await _planRepository.DeleteSectionGroup(sectionGroup);
            }

            foreach(var member in members)
            {
                await _groupRepository.DeleteGroupUser(member);
            }

            await _groupRepository.DeleteGroup(group);
        }
    }
}
