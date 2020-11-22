using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Training.Api.Extensions;
using Amphibian.Patrol.Training.Api.Dtos;
using Microsoft.Extensions.Logging;
using Amphibian.Patrol.Training.Api.Infrastructure;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IPatrolService _patrolService;
        private IUserService _userService;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        

        public UserController(ILogger<UserController> logger, IPatrolService patrolService,IUserRepository userRepository,IEmailService emailService
            , IPatrolRepository patrolRepository, IGroupRepository groupRepository)
        {
            _logger = logger;
            _patrolService = patrolService;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        [Route("user/list/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> List(int patrolId)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), patrolId)).CanMaintainUsers())
            {
                var users = await _patrolRepository.GetUsersForPatrol(patrolId);
                return Ok(users);
            }
            else
            {
                return Forbid();
            }
        }

        public class AddUserDto
        {
            public int PatrolId { get; set; }
            public Role? Role { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }
        [HttpPost]
        [Route("user/add-to-patrol")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> AddToPatrol(AddUserDto dto)
        {
            if((await _patrolService.GetUserRoleInPatrol(User.GetUserId(),dto.PatrolId)).CanMaintainUsers())
            {
                _userService.AddUserToPatrol(dto.PatrolId, dto.Role, dto.FirstName, dto.LastName, dto.Email);
                return Ok();
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
        public async Task<IActionResult> RemoveFrompatrol(RemoveUserDto dto)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), dto.PatrolId)).CanMaintainUsers())
            {
                await _patrolRepository.DeletePatrolUser(dto.UserId, dto.PatrolId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        public class GroupUserDto
        {
            public int UserId { get; set; }
            public int GroupId { get; set; }
        }
        [HttpPost]
        [Route("user/add-user-to-group")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> AddUserToGroup(GroupUserDto dto)
        {
            var group = await _groupRepository.GetGroup(dto.GroupId);
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), group.PatrolId)).CanMaintainUsers())
            {
                await _userService.AddUserToGroup(dto.UserId, dto.GroupId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPost]
        [Route("user/remove-user-from-group")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> RemoveUserFromGroup(GroupUserDto dto)
        {
            var group = await _groupRepository.GetGroup(dto.GroupId);
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), group.PatrolId)).CanMaintainUsers())
            {
                await _userService.RemoveUserFromGroup(dto.UserId, dto.GroupId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
    }
}