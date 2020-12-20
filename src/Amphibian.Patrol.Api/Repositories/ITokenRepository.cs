using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface ITokenRepository
    {
        Task DeleteToken(Token token);
        Task<Token> GetToken(Guid tokenGuid);
        Task<Token> GetToken(int id);
        Task InsertToken(Token token);
        Task UpdateToken(Token token);
        Task SupersedeActiveTokensForUsers(IList<int> userIds,DateTime when);
    }
}