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

namespace Amphibian.Patrol.Training.Api.Infrastructure
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserRepository _userRepository;
        private readonly TokenRepository _tokenRepository;
        private readonly PasswordService _passwordService;
       
        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            TokenRepository tokenRepository,
            UserRepository userRepository,
            PasswordService passwordService)
            : base(options, logger, encoder, clock)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _passwordService = passwordService;
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
                    var checkUser = await _userRepository.GetUser(email);
                    if(_passwordService.CheckPassword(checkUser, password))
                    {
                        //success
                        user = checkUser;
                    }
                    else
                    {
                        //bad password
                        return AuthenticateResult.Fail("Invalid Username or Password");
                    }
                }
                else if(Guid.TryParse(credentialString,out tokenGuid))
                {
                    var token = await _tokenRepository.GetToken(tokenGuid);
                    if(token==null)
                    {
                        //invalid token
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }
                    else if(DateTime.Now - token.LastRequestAt > new TimeSpan(1,0,0))
                    {
                        //expired token
                        await _tokenRepository.DeleteToken(token);
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }
                    else
                    {
                        //valid token
                        user = await _userRepository.GetUser(token.UserId);

                        token.LastRequestAt = DateTime.Now;
                        await _tokenRepository.UpdateToken(token);
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
