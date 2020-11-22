using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly UserRepository _userRepository;

        public AuthenticationController(ILogger<AuthenticationController> logger, AuthenticationService authenticationService, UserRepository userRepository)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userRepository = userRepository;
        }

        public class AuthenticationRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        [HttpPost]
        [Route("user/authenticate")]
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

    }
}
