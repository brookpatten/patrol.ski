using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class PatrolController : ControllerBase
    {
        private readonly ILogger<PatrolController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPatrolService _patrolService;
        private IPatrolCreationService _patrolCreationService;
        private IUserRepository _userRepository;

        public PatrolController(ILogger<PatrolController> logger, IPatrolRepository patrolRepository, IPatrolCreationService patrolCreationService, IPatrolService patrolService, IUserRepository userRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolCreationService = patrolCreationService;
            _patrolService = patrolService;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("patrol/create/empty")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateEmptyPatrol(string name)
        {
            var patrol = await _patrolCreationService.CreateNewPatrol(User.GetUserId(), name);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            return Ok(patrols);
        }

        [HttpPost]
        [Route("patrol/create/default")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateDefaultPatrol(string name)
        {
            var patrol = await _patrolCreationService.CreateNewPatrol(User.GetUserId(), name);
            await _patrolCreationService.CreateDefaultInitialSetup(patrol.Id);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            return Ok(patrols);
        }

        [HttpPost]
        [Route("patrol/create/demo")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateDemoPatrol(string name)
        {
            var patrol = await _patrolCreationService.CreateNewPatrol(User.GetUserId(), name);

            var user = await _userRepository.GetUser(User.GetUserId());
            
            await _patrolCreationService.CreateDemoInitialSetup(patrol, user);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            return Ok(patrols);
        }

        [HttpPost]
        [Route("patrol")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Update(Amphibian.Patrol.Api.Models.Patrol patrol)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), patrol.Id)).CanmaintainPatrol())
            {
                await _patrolRepository.UpdatePatrol(patrol);
                var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
                return Ok(patrols);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
