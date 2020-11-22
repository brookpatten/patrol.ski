using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly UserRepository _userRepository;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(ILogger<ScheduleController> logger, UserRepository userRepository, AuthenticationService authenticationService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("user/authenticate")]
        public async Task Authenticate(string email,string password)
        {
            var user = await _userRepository.GetUser(email);
            if(user!=null)
            {
                if(_authenticationService.CheckPassword(user,password))
                {
                    Ok(new
                    {
                        user = new
                        {
                            user.Id,
                            user.Email,
                            user.FirstName,
                            user.LastName
                        },
                        token = Guid.NewGuid()
                    });
                }
                else
                {
                    BadRequest(new { message = "Username or password is incorrect" });
                }
            }
            else
            {
                BadRequest(new { message = "Username or password is incorrect" });
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
        [AllowAnonymous]
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
                user = new User()
                {
                    Email = registration.Email,
                    FirstName = registration.First,
                    LastName = registration.Last
                };
                _authenticationService.SetPassword(user, registration.Password);
                await _userRepository.InsertUser(user);
                return Ok(new
                {
                    Email = user.Email,
                    First = user.FirstName,
                    Last = user.LastName,
                    Token = Guid.NewGuid()
                });
            }
        }

    }
}
