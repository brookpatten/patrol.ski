using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class UserService: IUserService
    {
        private ILogger<UserService> _logger;
        private IUserRepository _userRepository;
        private IEmailService _emailService;
        private IGroupRepository _groupRepository;
        private IPatrolRepository _patrolRepository;

        public UserService(ILogger<UserService> logger,IUserRepository userRepository, IEmailService emailService, IGroupRepository groupRepository, IPatrolRepository patrolRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _emailService = emailService;
            _groupRepository = groupRepository;
            _patrolRepository = patrolRepository;
        }

        public async Task<User> AddUserToPatrol(int patrolId, Role? role,string firstName,string lastName,string email)
        {
            var user = await _userRepository.GetUser(email);
            if (user == null)
            {
                //create and email
                user = new User()
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                };
                await _userRepository.InsertUser(user);
                await _emailService.SendNewUserWelcomeEmail(user, "", "");
            }
            else
            {
                await _emailService.SendExistingUserWelcomeEmail(user, "", "");
            }

            var patrolUser = new PatrolUser()
            {
                PatrolId = patrolId,
                UserId = user.Id,
                Role = role
            };

            await _patrolRepository.InsertPatrolUser(patrolUser);
            return user;
        }

        public async Task AddUserToGroup(int userId, int groupId)
        {
            var group = await _groupRepository.GetGroup(groupId);
            var groups = await _groupRepository.GetGroupsForUser(group.PatrolId, userId);
            if (!groups.Any(x => x.Id == groupId))
            {
                var groupUser = new GroupUser()
                {
                    GroupId = groupId,
                    UserId = userId
                };
                await _groupRepository.InsertGroupUser(groupUser);
            }
        }

        public async Task RemoveUserFromGroup(int userId, int groupId)
        {
            var gu = await _groupRepository.GetGroupUser(userId,groupId);

            if (gu != null)
            {
                await _groupRepository.DeleteGroupUser(gu);
            }
        }
    }
}
