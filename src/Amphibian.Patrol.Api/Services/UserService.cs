using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public class UserService : IUserService
    {
        private ILogger<UserService> _logger;
        private IUserRepository _userRepository;
        private IEmailService _emailService;
        private IGroupRepository _groupRepository;
        private IPatrolRepository _patrolRepository;
        private IMapper _mapper;

        public UserService(ILogger<UserService> logger, IUserRepository userRepository, IEmailService emailService, IGroupRepository groupRepository, IPatrolRepository patrolRepository, IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _emailService = emailService;
            _groupRepository = groupRepository;
            _patrolRepository = patrolRepository;
            _mapper = mapper;
        }

        public async Task<User> AddUserToPatrol(int patrolId, Role? role, string firstName, string lastName, string email)
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
            var gu = await _groupRepository.GetGroupUser(userId, groupId);

            if (gu != null)
            {
                await _groupRepository.DeleteGroupUser(gu);
            }
        }

        public async Task<IEnumerable<PatrolUserDto>> GetPatrolUsers(int patrolId)
        {
            var users = await _patrolRepository.GetPatrolUsersForPatrol(patrolId);
            var patrolUsers = await _userRepository.GetUsers(users.Select(x => x.UserId).Distinct().ToList());
            var groupUsers = await _groupRepository.GetGroupsForUsers(patrolId, users.Select(x => x.UserId).Distinct().ToList());
            var groups = await _groupRepository.GetGroupsForPatrol(patrolId);

            var dtos = _mapper.Map<IEnumerable<User>, IEnumerable<PatrolUserDto>>(patrolUsers);

            foreach(var dto in dtos)
            {
                dto.PatrolUserId = users.Single(x => x.UserId == dto.Id).Id;
                dto.PatrolId = patrolId;
                dto.Role = users.Single(x => x.UserId == dto.Id).Role;
                dto.Groups = groupUsers.Where(x => x.UserId == dto.Id).Select(x => groups.Single(y => y.Id == x.GroupId)).ToList();
            }

            dtos = dtos.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            return dtos;
        }

        public async Task<PatrolUserDto> GetPatrolUser(int patrolId, int userId)
        {
            var patrolUser = await _patrolRepository.GetPatrolUser(userId, patrolId);
            var user = await _userRepository.GetUser(userId);
            var groupUsers = await _groupRepository.GetGroupsForUser(patrolId, userId);
            
            var dto = _mapper.Map<User, PatrolUserDto>(user);

            dto.PatrolUserId = patrolUser.Id;
            dto.PatrolId = patrolId;
            dto.Role = patrolUser.Role;
            dto.Groups = groupUsers.ToList();

            return dto;
        }

        public async Task UpdatePatrolUser(PatrolUserDto dto)
        {
            var newEmailUser = await _userRepository.GetUser(dto.Email);

            if (newEmailUser == null || newEmailUser.Id == dto.Id)
            {
                var user = await _userRepository.GetUser(dto.Id);
                user.Email = dto.Email;
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                await _userRepository.UpdateUser(user);
                //note admins cannot change notification preferences for users, on purpose

                var patrolUser = await _patrolRepository.GetPatrolUser(dto.Id, dto.PatrolId);
                patrolUser.Role = dto.Role;
                await _patrolRepository.UpdatePatrolUser(patrolUser);

                var existingGroupUsers = await _groupRepository.GetGroupUsersForUser(dto.PatrolId, dto.Id);

                await existingGroupUsers.DifferenceWith(dto.Groups
                , (e, c) => e.GroupId == c.Id
                , c => _groupRepository.InsertGroupUser(new GroupUser() { UserId = dto.Id, GroupId = c.Id })
                , e => _groupRepository.DeleteGroupUser(e));
            }
            else
            {
                throw new InvalidOperationException("Email in use");
            }
        }
    }
}