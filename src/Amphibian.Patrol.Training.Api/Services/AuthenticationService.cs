using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Models;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class AuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly TokenRepository _tokenRepository;
        private readonly ISystemClock _systemClock;

        public AuthenticationService(ILogger<AuthenticationService> logger, UserRepository userRepository, PasswordService passwordService, TokenRepository tokenRepository, ISystemClock systemClock)
        {
            _logger = logger;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenRepository = tokenRepository;
            _systemClock = systemClock;
        }

        public async Task<Token> CreateNewTokenForUser(User user)
        {
            var now = _systemClock.UtcNow.UtcDateTime;
            var newToken = new Token()
            {
                CreatedAt = now,
                LastRequestAt = now,
                TokenGuid = Guid.NewGuid(),
                UserId = user.Id
            };
            await _tokenRepository.InsertToken(newToken);
            return newToken;
        }

        public async Task<User> AuthenticateUserWithToken(Guid guid)
        {
            var token = await _tokenRepository.GetToken(guid);
            if (token == null)
            {
                //invalid token
                return null;
            }
            else if (DateTime.Now - token.LastRequestAt > new TimeSpan(1, 0, 0))
            {
                //expired token
                await _tokenRepository.DeleteToken(token);
                return null;
            }
            else
            {
                //valid token
                var user = await _userRepository.GetUser(token.UserId);

                token.LastRequestAt = _systemClock.UtcNow.UtcDateTime;
                await _tokenRepository.UpdateToken(token);

                return user;
            }
        }

        public async Task<User> AuthenticateUserWithPassword(string email,string password)
        {
            var checkUser = await _userRepository.GetUser(email);
            if (checkUser != null)
            {
                if (_passwordService.CheckPassword(checkUser, password))
                {
                    return checkUser;
                }
                else
                {
                    //bad password
                    return null;
                }
            }
            else
            {
                //not found with email
                return null;
            }
        }

        public async Task<User> RegisterUser(string email, string first, string last, string password)
        {
            var user = new User()
            {
                Email = email,
                FirstName = first,
                LastName = last
            };
            _passwordService.SetPassword(user, password);
            await _userRepository.InsertUser(user);
            return user;
        }
    }
}
