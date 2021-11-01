using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Dtos;
using Microsoft.Extensions.Logging;
using Amphibian.Patrol.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using AutoMapper;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IPatrolService _patrolService;
        private IUserService _userService;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        private IPlanRepository _planRepository;
        private IUserRepository _userRepository;
        private Services.IAuthenticationService _authenticationService;
        private ITokenRepository _tokenRepository;
        private ISystemClock _systemClock;
        private IMapper _mapper;
        

        public UserController(ILogger<UserController> logger, IPatrolService patrolService,IUserRepository userRepository,IEmailService emailService
            , IPatrolRepository patrolRepository, IGroupRepository groupRepository, IUserService userService, IPlanRepository planRepository
            , Services.IAuthenticationService authenticationService, ITokenRepository tokenRepository, ISystemClock systemClock, IMapper mapper)
        {
            _logger = logger;
            _patrolService = patrolService;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
            _userService = userService;
            _planRepository = planRepository;
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _tokenRepository = tokenRepository;
            _systemClock = systemClock;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("user/list/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> List(int patrolId)
        {
            if (User.RoleInPatrol( patrolId).CanMaintainUsers())
            {
                var users = await _userService.GetPatrolUsers(patrolId);
                return Ok(users);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("user/{patrolId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> Get(int patrolId, int userId)
        {
            if (User.RoleInPatrol( patrolId).CanMaintainUsers())
            {
                var user = await _userService.GetPatrolUser(patrolId,userId);
                return Ok(user);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("user")]
        [Authorize]
        public async Task<IActionResult> GetSelf()
        {
            var user = await _userRepository.GetUser(User.UserId());
            var id = _mapper.Map<User, UserIdentifier>(user);

            return Ok(id);
        }

        [HttpGet]
        [Route("user/groups/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetGroups(int patrolId)
        {
            var role = User.RoleInPatrol( patrolId);
            if (role.CanMaintainUsers()
                || role.CanMaintainSchedule())
            {
                var groups = await _groupRepository.GetGroupsForPatrol(patrolId);
                return Ok(groups);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("user/group/{groupId}")]
        [Authorize]
        public async Task<IActionResult> GetGroup(int groupId)
        {
            var group = await _groupRepository.GetGroup(groupId);
            if (User.RoleInPatrol( group.PatrolId).CanMaintainGroups())
            {
                var memebers = await _groupRepository.GetUsersInGroup(groupId);
                var plans = await _planRepository.GetPlansWithSectionsAllowedByGroup(groupId);
                return Ok(new
                {
                    Group = group,
                    Members = memebers,
                    Plans = plans
                });
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPut]
        [Route("user/group")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> UpdateGroup(Group newGroup)
        {
            Group existing = null;
            if(newGroup.Id==default(int))
            {
                existing = new Group()
                {
                    Name = newGroup.Name,
                    PatrolId = newGroup.PatrolId
                };
            }
            else
            {
                existing = await _groupRepository.GetGroup(newGroup.Id);
                if(existing.PatrolId!=newGroup.PatrolId)
                {
                    return Forbid();
                }
                existing.Name = newGroup.Name;
            }

            if (User.RoleInPatrol( newGroup.PatrolId).CanMaintainGroups())
            {
                if (existing.Id == default(int))
                {
                    await _groupRepository.InsertGroup(existing);
                }
                else
                {
                    await _groupRepository.UpdateGroup(existing);
                }

                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete]
        [Route("user/groups/{patrolId}/{groupId}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> RemoveGroup(int patrolId,int groupId)
        {
            if (User.RoleInPatrol( patrolId).CanMaintainGroups())
            {
                var groups = await _groupRepository.GetGroupsForPatrol(patrolId);
                if(groups.Any(y=>y.Id==groupId))
                {
                    await _patrolService.DeleteGroup(groupId);
                    return Ok();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        /// <summary>
        /// delete yourself and all your personal information
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("user")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Delete()
        {
            await _userService.RemovePersonalInformation(User.UserId());

            Response.SendNewToken("Deleted");

            return Ok();
        }

        [HttpPut]
        [Route("user")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Save(PatrolUserDto dto)
        {
            //users can update some things themselves
            if(dto.Id == User.UserId() && !dto.Role.HasValue && dto.Groups==null && dto.PatrolUserId==default(int))
            {
                var newEmailUser = await _userRepository.GetUser(dto.Email);

                if (newEmailUser == null || newEmailUser.Id == dto.Id)
                {
                    var user = await _userRepository.GetUser(dto.Id);

                    user.FirstName = dto.FirstName;
                    user.LastName = dto.LastName;
                    //user.Email = dto.Email;
                    user.AllowEmailNotifications = dto.AllowEmailNotifications;
                    user.NspNumber = dto.NspNumber;
                    user.ProfileImageUrl = dto.ProfileImageUrl;
                    await _userRepository.UpdateUser(user);
                }
                else
                {
                    throw new InvalidOperationException("Email in use");
                }
                return Ok();
            }
            //admins can update some things for people in their patrol
            else if (User.RoleInPatrol( dto.PatrolId).CanMaintainUsers())
            {
                //ensure the groups specified match the specified patrol
                var validGroups = await _groupRepository.GetGroupsForPatrol(dto.PatrolId);
                if (dto.Groups.All(x => validGroups.Any(y => y.Id == x.Id)))
                {
                    if(dto.Id==default(int))
                    {
                        var user = await _userService.AddUserToPatrol(dto.PatrolId, dto.Role, dto.FirstName, dto.LastName, dto.Email);
                        dto.Id = user.Id;
                    }

                    await _userService.UpdatePatrolUser(dto);

                    //if it's the current user, send them a refreshed token
                    if (dto.Id == User.UserId())
                    {
                        Response.SendNewToken(await _authenticationService.IssueJwtToUser(User.UserId(), User.TokenGuid()));
                    }
                    else
                    {
                        await _tokenRepository.SupersedeActiveTokensForUsers(new List<int>() { dto.Id }, _systemClock.UtcNow.UtcDateTime);
                    }

                    return Ok();
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        public class RemoveUserDto
        {
            public int PatrolId { get; set; }
            public int UserId { get; set; }
        }
        [HttpPost]
        [Route("user/remove-from-patrol")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> RemoveFromPatrol(RemoveUserDto dto)
        {
            if (User.RoleInPatrol( dto.PatrolId).CanMaintainUsers())
            {
                await _patrolRepository.DeletePatrolUser(dto.PatrolId,dto.UserId);

                //TODO, if the user asn't current user, mark their tokens to supersede
                //refresh the users jwt to match the above change
                if (dto.UserId == User.UserId())
                {
                    Response.SendNewToken(await _authenticationService.IssueJwtToUser(User.UserId(), User.TokenGuid()));
                }
                else
                {
                    //supersede any tokens for the user
                    var patrolUsers = (await _patrolRepository.GetUsersForPatrol(dto.PatrolId)).ToList();
                    patrolUsers = patrolUsers.Where(x => x.Id != User.UserId()).ToList();
                    await _tokenRepository.SupersedeActiveTokensForUsers(patrolUsers.Select(x => x.Id).ToList(), _systemClock.UtcNow.UtcDateTime);
                }

                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
    }
}