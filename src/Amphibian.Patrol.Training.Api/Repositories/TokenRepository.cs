using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using Dapper;
using Dommel;

using System.Runtime.InteropServices.WindowsRuntime;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IDbConnection _connection;

        public TokenRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertToken(Token token)
        {
            token.Id = (int)await _connection.InsertAsync(token).ConfigureAwait(false);
        }

        public async Task UpdateToken(Token token)
        {
            await _connection.UpdateAsync(token).ConfigureAwait(false);
        }

        public async Task<Token> GetToken(int id)
        {
            var token = await _connection.GetAsync<Token>(id).ConfigureAwait(false);
            return token;
        }

        public async Task<Token> GetToken(Guid tokenGuid)
        {
            var tokens = await _connection.SelectAsync<Token>(x => x.TokenGuid == tokenGuid);
            if(tokens.Any())
            {
                return tokens.First();
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteToken(Token token)
        {
            await _connection.DeleteAsync(token).ConfigureAwait(false);
        }
    }
}
