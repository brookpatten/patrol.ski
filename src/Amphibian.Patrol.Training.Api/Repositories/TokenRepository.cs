using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using Dapper;
using Dapper.Contrib;

using Dapper.Contrib.Extensions;
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
            token.Id = await _connection.InsertAsync(token).ConfigureAwait(false);
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
            var token = await _connection.QuerySingleOrDefaultAsync<Token>(@"select Id,TokenGuid,UserId,CreatedAt,LastRequestAt from tokens where TokenGuid=@tokenGuid", new { tokenGuid }).ConfigureAwait(false);
            return token;
        }

        public async Task DeleteToken(Token token)
        {
            await _connection.DeleteAsync(token).ConfigureAwait(false);
        }
    }
}
