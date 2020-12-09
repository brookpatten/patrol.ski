﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Validations;
using FluentValidation.AspNetCore;
using FluentValidation;
using Amphibian.Patrol.Api.Infrastructure;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IEmailService _emailService;
        private readonly IPatrolCreationService _patrolCreationService;
        
        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService, 
            IUserRepository userRepository, IPatrolRepository patrolRepository, IEmailService emailService, IPatrolCreationService patrolCreationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _patrolRepository = patrolRepository;
            _emailService = emailService;
            _patrolCreationService = patrolCreationService;
        }

        public class AuthenticationRequest
        {
            public string email { get; set; }
            public string password { get; set; }
        }
        [HttpPost]
        [Route("user/authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            User user;
            user = await _authenticationService.AuthenticateUserWithPassword(request.email, request.password);

            if (user!=null)
            {
                var token = await _authenticationService.CreateNewTokenForUser(user);
                _logger.LogInformation("Authenticated {@email} via email/password OK, Created Token {@token}", user.Email, token);
                var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);
                return Ok(new
                {
                    User = (UserIdentifier)user,
                    Token = token.TokenGuid,
                    Patrols = patrols
                });
            }
            else
            {
                _logger.LogInformation("Username or password is incorrect");
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }

        public class RegistrationRequest
        {
            public string email { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string password { get; set; }
        }
        public class RegistrationResult
        {
            public string email { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public Guid token { get; set; }
        }
        [HttpPost]
        [Route("user/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegistrationRequest registration)
        {
            //it is assumed if we got to this point the validation completed and is valid
            var user = await _userRepository.GetUser(registration.email);

            user = await _authenticationService.RegisterUser(registration.email, registration.firstname, registration.lastname, registration.password);
            var token = await _authenticationService.CreateNewTokenForUser(user);
            var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);

            return Ok(new
            {
                User = (UserIdentifier)user,
                Token = token.TokenGuid,
                Patrols = patrols
            });
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

        public class ResetPasswordRequest
        {
            public string Email { get; set; }
        }
        [HttpPost]
        [Route("user/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;
            var user = await _userRepository.GetUser(request.Email);

            if (user!=null)
            {
                var token = await _authenticationService.CreateNewTokenForUser(user);
                await _emailService.SendResetEmail(user, $"user/recover-password/{token.TokenGuid}");
            }
            
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

        [HttpPost]
        [Route("user/authenticate-throwaway")]
        [AllowAnonymous]
        [UnitOfWork]
        public async Task<IActionResult> AuthenticateWithThrowaway()
        {
            try
            {
                var throwaway = await _patrolCreationService.CreateDemoUserAndPatrol();

                var token = await _authenticationService.CreateNewTokenForUser(throwaway.Item1);
                var patrols = await _patrolRepository.GetPatrolsForUser(throwaway.Item1.Id);
                return Ok(new
                {
                    User = (UserIdentifier)throwaway.Item1,
                    Token = token.TokenGuid,
                    Patrols = patrols
                });
            }
            catch(Exception ex)
            {
                return this.Problem();
            }
        }
    }
}
