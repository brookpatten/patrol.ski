using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Infrastructure;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Services;
using Amphibian.Patrol.Configuration;
using Facebook;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

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
        private readonly AuthenticationConfiguration _authenticationConfiguration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService, 
            IUserRepository userRepository, IPatrolRepository patrolRepository, IEmailService emailService, 
            IPatrolCreationService patrolCreationService, AuthenticationConfiguration authenticationConfiguration)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _patrolRepository = patrolRepository;
            _emailService = emailService;
            _patrolCreationService = patrolCreationService;
            _authenticationConfiguration = authenticationConfiguration;
        }

        [HttpGet]
        [Route("authentication/oauth2/configuration-js")]
        [Produces("application/javascript")]
        public async Task<IActionResult> GetJsClientIds()
        {
            var result = new
            {
                googleClientId = _authenticationConfiguration.Google.ClientId,
                facebookAppId = _authenticationConfiguration.Facebook.AppId,
                microsoftClientId = _authenticationConfiguration.Microsoft.ClientId,
                microsoftTenantId = _authenticationConfiguration.Microsoft.TenantId
            };
            var resultJson = JsonConvert.SerializeObject(result);
            var fullJson = $"var oauth2Configuration = {resultJson};";
            return Content(fullJson);
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

                var jwt = _authenticationService.CreateSignedJwtToken(token, user, patrols.ToList());

                return Ok(new
                {
                    User = (UserIdentifier)user,
                    Token = jwt,
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
            public string nspNumber { get; set; }
        }
        [HttpPost]
        [Route("user/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegistrationRequest registration)
        {
            //it is assumed if we got to this point the validation completed and is valid
            var user = await _userRepository.GetUser(registration.email);

            user = await _authenticationService.RegisterUser(registration.email, registration.firstname, registration.lastname, registration.password,registration.nspNumber);
            var token = await _authenticationService.CreateNewTokenForUser(user);
            var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);

            var jwt = _authenticationService.CreateSignedJwtToken(token, user, patrols.ToList());

            return Ok(new
            {
                User = (UserIdentifier)user,
                Token = jwt,
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
            var userId = User.UserId();
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

                var jwt = _authenticationService.CreateSignedJwtToken(token, throwaway.Item1, patrols.ToList());

                return Ok(new
                {
                    User = (UserIdentifier)throwaway.Item1,
                    Token = jwt,
                    Patrols = patrols
                });
            }
            catch(Exception ex)
            {
                return this.Problem();
            }
        }

        public class GoogleAuthentication
        {
            public string id_token { get; set; }
        }
        /// <summary>
        /// login using a google oauth2 id token
        /// </summary>
        /// <param name="googleAuth"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/authenticate/google")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateGoogle(GoogleAuthentication googleAuth)
        {
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(googleAuth.id_token);
            if(validPayload!=null)
            {
                if (validPayload.EmailVerified)
                {
                    string email = validPayload.Email;
                    string fam = validPayload.FamilyName;
                    string given = validPayload.GivenName;
                    string profile = validPayload.Picture;

                    var user = await _userRepository.GetUser(email);
                    if (user == null)
                    {
                        user = new User()
                        {
                            Email = email,
                            FirstName = given,
                            LastName = fam,
                            AllowEmailNotifications = true,
                            UsesGoogleAuth=true
                        };
                        await _userRepository.InsertUser(user);
                    }
                    else if(!user.UsesGoogleAuth)
                    {
                        user.UsesGoogleAuth = true;
                        await _userRepository.UpdateUser(user);
                    }

                    var token = await _authenticationService.CreateNewTokenForUser(user);
                    var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);

                    var jwt = _authenticationService.CreateSignedJwtToken(token, user, patrols.ToList());

                    return Ok(new
                    {
                        User = (UserIdentifier)user,
                        Token = jwt,
                        Patrols = patrols
                    });
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

        public class FacebookAuthentication
        {
            public string accessToken { get; set; }
        }
        /// <summary>
        /// login using a facebook oauth 2 access token
        /// </summary>
        /// <param name="facebookAuth"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/authenticate/facebook")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateFacebook(FacebookAuthentication facebookAuth)
        {
            var client = new FacebookClient(facebookAuth.accessToken);
            dynamic authenticatingUser = client.Get("me",new { fields="first_name,last_name,email,id"});

            if (authenticatingUser != null)
            {
                //if (validPayload.EmailVerified)
                {
                    string email = authenticatingUser.email;
                    string fam = authenticatingUser.last_name;
                    string given = authenticatingUser.first_name;
                    //string profile = authenticatingUser.Picture;

                    var user = await _userRepository.GetUser(email);
                    if (user == null)
                    {
                        user = new User()
                        {
                            Email = email,
                            FirstName = given,
                            LastName = fam,
                            AllowEmailNotifications = true,
                            UsesFacebookAuth=true
                        };
                        await _userRepository.InsertUser(user);
                    }
                    else if (!user.UsesFacebookAuth)
                    {
                        user.UsesFacebookAuth = true;
                        await _userRepository.UpdateUser(user);
                    }

                    var token = await _authenticationService.CreateNewTokenForUser(user);
                    var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);

                    var jwt = _authenticationService.CreateSignedJwtToken(token, user, patrols.ToList());

                    return Ok(new
                    {
                        User = (UserIdentifier)user,
                        Token = jwt,
                        Patrols = patrols
                    });
                }
                //else
                {
                    //return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        
        public class MicrosoftAuthentication
        {
            public string accessToken { get; set; }
        }
        /// <summary>
        /// login using a microsoft oauth2 accessToken
        /// </summary>
        [HttpPost]
        [Route("user/authenticate/microsoft")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateMicrosoft(MicrosoftAuthentication microsoftAuthentication)
        {
            var url = $"{_authenticationConfiguration.Microsoft.GraphBaseEndpoint}/v1.0/me";
            var client = new HttpClient();
            var defaultRequestHeaders = client.DefaultRequestHeaders;
            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", microsoftAuthentication.accessToken);

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject result = JsonConvert.DeserializeObject(json) as JObject;

                var firstName = result["givenName"].Value<string>();
                var lastName = result["surname"].Value<string>();
                var email = result["mail"].Value<string>();
                if(string.IsNullOrEmpty(email))
                {
                    email = result["userPrincipalName"].Value<string>();
                }

                var user = await _userRepository.GetUser(email);
                if (user == null)
                {
                    user = new User()
                    {
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        AllowEmailNotifications = true,
                        UsesMicrosoftAuth = true
                    };
                    await _userRepository.InsertUser(user);
                }
                else if (!user.UsesMicrosoftAuth)
                {
                    user.UsesMicrosoftAuth = true;
                    await _userRepository.UpdateUser(user);
                }

                var token = await _authenticationService.CreateNewTokenForUser(user);
                var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);

                var jwt = _authenticationService.CreateSignedJwtToken(token, user, patrols.ToList());

                return Ok(new
                {
                    User = (UserIdentifier)user,
                    Token = jwt,
                    Patrols = patrols
                });
            }
            else
            {
                return Forbid();
            }
        }
    }
}
