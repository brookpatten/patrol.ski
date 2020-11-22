using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface ITokenRepository
    {
        Task DeleteToken(Token token);
        Task<Token> GetToken(Guid tokenGuid);
        Task<Token> GetToken(int id);
        Task InsertToken(Token token);
        Task UpdateToken(Token token);
    }
}