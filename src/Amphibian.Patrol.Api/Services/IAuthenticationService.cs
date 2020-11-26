using Amphibian.Patrol.Api.Models;
using System;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IAuthenticationService
    {
        Task<User> AuthenticateUserWithPassword(string email, string password);
        Task<User> AuthenticateUserWithToken(Guid guid);
        Task<Token> CreateNewTokenForUser(User user);
        Task<User> RegisterUser(string email, string first, string last, string password);
        Task ChangePassword(string userEmail, string password);
    }
}