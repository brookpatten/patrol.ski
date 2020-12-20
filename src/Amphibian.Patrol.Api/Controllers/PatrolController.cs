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
using IAuthenticationService = Amphibian.Patrol.Api.Services.IAuthenticationService;

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
        private IAuthenticationService _authenticationService;
        private ITokenRepository _tokenRepository;
        private ISystemClock _systemClock;

        public PatrolController(ILogger<PatrolController> logger, IPatrolRepository patrolRepository, 
            IPatrolCreationService patrolCreationService, IPatrolService patrolService, IUserRepository userRepository,
            IAuthenticationService authenticationService, ITokenRepository tokenRepository, ISystemClock systemClock)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolCreationService = patrolCreationService;
            _patrolService = patrolService;
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _tokenRepository = tokenRepository;
            _systemClock = systemClock;
        }

        [HttpPost]
        [Route("patrol/create/empty")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateEmptyPatrol(Models.Patrol patrolSetup)
        {
            //TODO: update to also update jwt
            var patrol = await _patrolCreationService.CreateNewPatrol(User.UserId(), patrolSetup);
            //TODO: we don't need this once the UI uses the JWT
            var patrols = await _patrolRepository.GetPatrolsForUser(User.UserId());

            //refresh the users jwt to match the above change
            Response.SendNewToken(await _authenticationService.IssueJwtToUser(User.UserId(), User.TokenGuid()));

            return Ok(patrols);
        }

        [HttpPost]
        [Route("patrol/create/default")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateDefaultPatrol(Models.Patrol patrolSetup)
        {
            //TODO: update to also update jwt
            var patrol = await _patrolCreationService.CreateNewPatrol(User.UserId(), patrolSetup);
            await _patrolCreationService.CreateDefaultInitialSetup(patrol.Id);
            //TODO: we don't need this once the UI uses the JWT
            var patrols = await _patrolRepository.GetPatrolsForUser(User.UserId());

            //refresh the users jwt to match the above change
            Response.SendNewToken(await _authenticationService.IssueJwtToUser(User.UserId(), User.TokenGuid()));

            return Ok(patrols);
        }

        [HttpPost]
        [Route("patrol/create/demo")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateDemoPatrol(Models.Patrol patrolSetup)
        {
            //TODO: update to also update jwt
            var patrol = await _patrolCreationService.CreateNewPatrol(User.UserId(), patrolSetup);

            var user = await _userRepository.GetUser(User.UserId());
            
            await _patrolCreationService.CreateDemoInitialSetup(patrol, user);
            //TODO: we don't need this once the UI uses the JWT
            var patrols = await _patrolRepository.GetPatrolsForUser(User.UserId());

            //refresh the users jwt to match the above change
            Response.SendNewToken(await _authenticationService.IssueJwtToUser(User.UserId(), User.TokenGuid()));


            return Ok(patrols);
        }

        [HttpPost]
        [Route("patrol")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Update(Amphibian.Patrol.Api.Models.Patrol patrol)
        {
            if (User.RoleInPatrol( patrol.Id).CanmaintainPatrol())
            {
                await _patrolRepository.UpdatePatrol(patrol);
                //TODO: we don't need this once the UI uses the JWT
                var patrols = await _patrolRepository.GetPatrolsForUser(User.UserId());

                //refresh the users jwt to match the above change
                Response.SendNewToken(await _authenticationService.IssueJwtToUser(User.UserId(), User.TokenGuid()));

                var patrolUsers = (await _patrolRepository.GetUsersForPatrol(patrol.Id)).ToList();
                patrolUsers = patrolUsers.Where(x => x.Id != User.UserId()).ToList();
                await _tokenRepository.SupersedeActiveTokensForUsers(patrolUsers.Select(x => x.Id).ToList(), _systemClock.UtcNow.UtcDateTime);

                return Ok(patrols);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
