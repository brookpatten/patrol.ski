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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Amphibian.Patrol.Training.Api.Extensions;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly IPatrolRepository _patrolRepository;
        
        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService, 
            IUserRepository userRepository, IPatrolRepository patrolRepository)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _patrolRepository = patrolRepository;
        }

        public class AuthenticationRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        [HttpPost]
        [Route("user/authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            var user = await _authenticationService.AuthenticateUserWithPassword(request.Email, request.Password);
            if(user!=null)
            {
                var token = await _authenticationService.CreateNewTokenForUser(user);
                return Ok(new
                {
                    Email = user.Email,
                    First = user.FirstName,
                    Last = user.LastName,
                    Token = token.TokenGuid
                });
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }

        public class RegistrationRequest
        {
            public string Email { get; set; }
            public string First { get; set; }
            public string Last { get; set; }
            public string Password { get; set; }
        }
        public class RegistrationResult
        {
            public string Email { get; set; }
            public string First { get; set; }
            public string Last { get; set; }
            public Guid Token { get; set; }
        }
        [HttpPost]
        [Route("user/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegistrationRequest registration)
        {
            var user = await _userRepository.GetUser(registration.Email);
            
            if (user != null)
            {
                return BadRequest(new { message = "User Already Exists" });
            }
            else
            {
                user = await _authenticationService.RegisterUser(registration.Email, registration.First, registration.Last, registration.Password);
                var token = await _authenticationService.CreateNewTokenForUser(user);

                return Ok(new
                {
                    Email = user.Email,
                    First = user.FirstName,
                    Last = user.LastName,
                    Token = token.TokenGuid
                });
            }
        }

        public class ChangePasswordRequest
        {
            public string Password { get; set; }
        }

        [HttpPost]
        [Route("user/password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;
            await _authenticationService.ChangePassword(email, request.Password);
            return Ok();
        }

        [HttpGet]
        [Route("user/patrols")]
        [Authorize]
        public async Task<IActionResult> GetPatrols()
        {
            var userId = User.GetUserId();
            var patrols = await _patrolRepository.GetPatrolsForUser(userId);
            return Ok(patrols);
        }

    }
}
