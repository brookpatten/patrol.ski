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
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            User user = null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentialString = Encoding.UTF8.GetString(credentialBytes);
                Guid tokenGuid;

                if(credentialString.Contains(":"))
                {
                    var credentials = credentialString.Split(new[] { ':' }, 2);
                    var email = credentials[0];
                    var password = credentials[1];
                    user = await _authenticationService.AuthenticateUserWithPassword(email, password);
                    if(user==null)
                    {
                        //bad password
                        return AuthenticateResult.Fail("Invalid Username or Password");
                    }
                }
                else if(Guid.TryParse(credentialString,out tokenGuid))
                {
                    user = await _authenticationService.AuthenticateUserWithToken(tokenGuid);
                    if(user==null)
                    {
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }
                }
                else
                {
                    //it's not a token and it's not a jwt, what is it?
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }
            }
            catch
            {
                //some exception we weren't ready for
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (user == null)
            {
                //this shouldn't actually happen, but just in case
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
