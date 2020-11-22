using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Training.Api.Extensions;
using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class PatrolController : ControllerBase
    {
        private readonly ILogger<PatrolController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private IPatrolCreationService _patrolCreationService;

        public PatrolController(ILogger<PatrolController> logger, IPatrolRepository patrolRepository, IPatrolCreationService patrolCreationService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolCreationService = patrolCreationService;
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
            await _patrolCreationService.CreateDemoInitialSetup(patrol.Id);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            return Ok(patrols);
        }
    }
}
