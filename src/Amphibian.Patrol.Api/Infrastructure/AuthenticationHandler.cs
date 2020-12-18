using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System;
using System.Text;
using System.Security.Claims;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Amphibian.Patrol.Api.Repositories;
using IAuthenticationService = Amphibian.Patrol.Api.Services.IAuthenticationService;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Amphibian.Patrol.Api.Dtos;

namespace Amphibian.Patrol.Api.Infrastructure
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPatrolRepository _patrolRepository;

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthenticationService authenticationService, IPatrolRepository patrolRepository)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
            _patrolRepository = patrolRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Logger.LogError("Missing Authorization Header");
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            ClaimsPrincipal principal=null;

            try
            {
                AuthenticationHeaderValue authenticationHeader;
                
                if (AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out authenticationHeader))
                {
                    if (authenticationHeader.Scheme == "Basic")
                    {
                        var credentialBytes = Convert.FromBase64String(authenticationHeader.Parameter);
                        var credentialString = Encoding.UTF8.GetString(credentialBytes);
                        
                        if (credentialString.Contains(":"))
                        {
                            var credentials = credentialString.Split(new[] { ':' }, 2);
                            var email = credentials[0];
                            var password = credentials[1];
                            var user = await _authenticationService.AuthenticateUserWithPassword(email, password);
                            if (user == null)
                            {
                                //bad password
                                Logger.LogError("Invalid Username or password");
                                return AuthenticateResult.Fail("Invalid Username or Password");
                            }
                            else
                            {
                                //make a token and put it in the response header?
                                
                                var token = await _authenticationService.CreateNewTokenForUser(user);
                                Logger.LogInformation("Authenticated User {@email} via email/password, created token {@token}", user.Email, token );

                                var patrols = await _patrolRepository.GetPatrolsForUser(user.Id);
                                var jwt = _authenticationService.CreateSignedJwtToken(token, user, patrols.ToList());
                                principal = _authenticationService.ValidateSignedJwtToken(jwt);

                                Response.Headers.Add("Authorization", "Token " + jwt);
                            }
                        }
                    }
                    else if(authenticationHeader.Scheme=="Token" || authenticationHeader.Scheme == "Bearer") //TODO: remove "Token"
                    {
                        if ((new JwtSecurityTokenHandler()).CanReadToken(authenticationHeader.Parameter))
                        {
                            principal = _authenticationService.ValidateSignedJwtToken(authenticationHeader.Parameter);
                            var parsed = principal.ParseAllClaims();
                            Logger.LogInformation("Authenticated User {@id} via bearer token {@tokenGuid}", parsed.User.Id, parsed.Token.TokenGuid );
                        }
                    }
                    else
                    {
                        Logger.LogError("Invalid Authorization Header");
                        //it's not a token and it's not a basic, what is it?
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }
                }
                else
                {
                    Logger.LogError("Invalid Authorization Header");
                    //we can't understand the auth header
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }
            }
            catch(Exception ex)
            {
                Logger.LogError("Exception in Authorization {@ex}",ex);
                //some exception we weren't ready for, likely a malformed header
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (principal == null)
            {
                //this shouldn't actually happen, but just in case
                Logger.LogError("Invalid Username or Password");
                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
