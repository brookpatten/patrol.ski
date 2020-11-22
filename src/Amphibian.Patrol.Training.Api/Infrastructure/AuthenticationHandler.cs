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

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;
using Amphibian.Patrol.Training.Api.Repositories;
using IAuthenticationService = Amphibian.Patrol.Training.Api.Services.IAuthenticationService;
using Microsoft.AspNetCore.Http;

namespace Amphibian.Patrol.Training.Api.Infrastructure
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthenticationService authenticationService)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                Logger.LogError("Missing Authorization Header");
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            User user = null;
            try
            {
                AuthenticationHeaderValue authenticationHeader;
                Guid tokenGuid;

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
                            user = await _authenticationService.AuthenticateUserWithPassword(email, password);
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
                                Response.Headers.Add("Authorization", "Token " + token.TokenGuid);
                            }
                        }
                    }
                    else if(authenticationHeader.Scheme=="Token" || authenticationHeader.Scheme == "Bearer") //TODO: remove "Token"
                    {
                        if(Guid.TryParse(authenticationHeader.Parameter,out tokenGuid))
                        {
                            user = await _authenticationService.AuthenticateUserWithToken(tokenGuid);
                            if (user == null)
                            {
                                Logger.LogError("Invalid Bearer Token");
                                return AuthenticateResult.Fail("Invalid Authorization Header");
                            }
                            Logger.LogInformation("Authenticated User {@email} via bearer token {@tokenGuid}", user.Email, tokenGuid );
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

            if (user == null)
            {
                //this shouldn't actually happen, but just in case
                Logger.LogError("Invalid Username or Password");
                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            

            return AuthenticateResult.Success(ticket);
        }
    }
}
