using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using Dapper;
using Dommel;

using Amphibian.Patrol.Training.Api.Models;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertUser(User user)
        {
            user.Id = (int)await _connection.InsertAsync(user).ConfigureAwait(false);
        }

        public async Task UpdateUser(User user)
        {
            await _connection.UpdateAsync(user).ConfigureAwait(false);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _connection.GetAsync<User>(id).ConfigureAwait(false);
            return user;
        }

        public async Task<User> GetUser(string email)
        {
            var user = await _connection.QuerySingleOrDefaultAsync<User>(@"select id,email,firstname,lastname,passwordsalt,passwordhash,passwordhashiterations from users where email=@email", new { email }).ConfigureAwait(false);
            return user;
        }
    }
}
