﻿using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IAuthenticationService
    {
        Task<User> AuthenticateUserWithPassword(string email, string password);
        Task<User> AuthenticateUserWithToken(Guid guid);
        Task<Token> CreateNewTokenForUser(User user);
        Task<User> RegisterUser(string email, string first, string last, string password, string nspNumber);
        Task ChangePassword(User user, string password);
        ClaimsPrincipal ValidateSignedJwtToken(string jwt);
        string CreateSignedJwtToken(Token token, UserIdentifier user, List<CurrentUserPatrolDto> patrols, bool minimal = false);
        Task<string> IssueJwtToUser(int userId, Guid? existingToken = null, bool minimal = false);
    }
}