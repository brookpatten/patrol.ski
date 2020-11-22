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
                        user.Id,
                        user.Email,
                        user.FirstName,
                        user.LastName
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

        [AllowAnonymous]
        [HttpPost]
        [Route("user/ceate")]
        public async Task Create(string email,string firstname,string lastname, string password)
        {
            var user = await _userRepository.GetUser(email);
            
            if (user != null)
            {
                BadRequest(new { message = "User Already Exists" });
            }
            else
            {
                user = new User()
                {
                    Email = email,
                    FirstName = firstname,
                    LastName = lastname
                };
                _authenticationService.SetPassword(user, password);
                await _userRepository.InsertUser(user);
                Ok(user);
            }
        }

    }
}
