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
        private readonly ITokenRepository _tokenRepository;
        private readonly ISystemClock _systemClock;

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthenticationService authenticationService, ITokenRepository tokenRepositry)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
            _tokenRepository = tokenRepositry;
            _systemClock = clock;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var now = _systemClock.UtcNow.UtcDateTime;
            if (!Request.Headers.ContainsKey("Authorization") && !Request.Cookies.ContainsKey("access_token"))
            {
                Logger.LogError("Missing Authorization Header or Cookie");
                return AuthenticateResult.Fail("Missing Authorization Header or Cookie");
            }
            
            ClaimsPrincipal principal=null;

            if (Request.Headers.ContainsKey("Authorization"))
            {
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

                                    Logger.LogInformation("Authenticated User {@email} via email/password", user.Email);

                                    var jwt = await _authenticationService.IssueJwtToUser(user.Id);
                                    principal = _authenticationService.ValidateSignedJwtToken(jwt);

                                    Response.Headers.Add("Authorization", "Token " + jwt);
                                }
                            }
                        }
                        else if (authenticationHeader.Scheme == "Token" || authenticationHeader.Scheme == "Bearer") //TODO: remove "Token"
                        {
                            if ((new JwtSecurityTokenHandler()).CanReadToken(authenticationHeader.Parameter))
                            {
                                var jwtPrincipal = _authenticationService.ValidateSignedJwtToken(authenticationHeader.Parameter);
                                var parsed = jwtPrincipal.ParseAllClaims();

                                //TODO: update this so that only superseded/expired token are in the db, "good" tokens will not be
                                var token = await _tokenRepository.GetToken(parsed.Token.TokenGuid);
                                if (token == null || (token.ExpiredAt.HasValue && token.ExpiredAt < now))
                                {
                                    //note we do NOT set the principle, it's expired so we let it fail as invalid
                                }
                                else if ((parsed.Minimal.HasValue && parsed.Minimal.Value) || (token.SupersededAt.HasValue && token.SupersededAt < now))
                                {
                                    //token is valid, but needs to be updated due to some change that was made to data contained therein
                                    //OR the token is a minimal one which needs to be repalced with a "full" token
                                    var refreshedJwt = await _authenticationService.IssueJwtToUser(parsed.User.Id, token.TokenGuid);
                                    //send the updated token back to client
                                    Response.Headers.Add("Authorization", "Token " + refreshedJwt);

                                    principal = _authenticationService.ValidateSignedJwtToken(refreshedJwt);
                                }
                                else
                                {
                                    principal = jwtPrincipal;
                                }

                                Logger.LogInformation("Authenticated User {@id} via bearer token {@tokenGuid}", parsed.User.Id, parsed.Token.TokenGuid);
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
                catch (Exception ex)
                {
                    Logger.LogError("Exception in Authorization {@ex}", ex);
                    //some exception we weren't ready for, likely a malformed header
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }
            }

            if(Request.Cookies.ContainsKey("access_token"))
            {
                try
                {
                    var jwtPrincipal = _authenticationService.ValidateSignedJwtToken(Request.Cookies["access_token"]);
                    var parsed = jwtPrincipal.ParseAllClaims();

                    //TODO: update this so that only superseded/expired token are in the db, "good" tokens will not be
                    var token = await _tokenRepository.GetToken(parsed.Token.TokenGuid);
                    if (token == null || (token.ExpiredAt.HasValue && token.ExpiredAt < now))
                    {
                        //note we do NOT set the principle, it's expired so we let it fail as invalid
                    }
                    else if ((parsed.Minimal.HasValue && parsed.Minimal.Value) || (token.SupersededAt.HasValue && token.SupersededAt < now))
                    {
                        //token is valid, but needs to be updated due to some change that was made to data contained therein
                        //OR the token is a minimal one which needs to be repalced with a "full" token
                        var refreshedJwt = await _authenticationService.IssueJwtToUser(parsed.User.Id, token.TokenGuid);
                        //send the updated token back to client
                        Response.Headers.Add("Authorization", "Token " + refreshedJwt);

                        principal = _authenticationService.ValidateSignedJwtToken(refreshedJwt);
                    }
                    else
                    {
                        principal = jwtPrincipal;
                    }

                    Logger.LogInformation("Authenticated User {@id} via cookie {@tokenGuid}", parsed.User.Id, parsed.Token.TokenGuid);
                }
                catch(Exception ex)
                {
                    Logger.LogError("Exception in Authorization {@ex}", ex);
                    //some exception we weren't ready for, likely a malformed header
                    return AuthenticateResult.Fail("Invalid Authorization Cookie");
                }
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
