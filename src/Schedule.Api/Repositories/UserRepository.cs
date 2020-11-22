using Schedule.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using Dapper;
using Dapper.Contrib;

using Dapper.Contrib.Extensions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Schedule.Api.Repositories
{
    public class UserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertUser(User user)
        {
            user.Id = await _connection.InsertAsync(user);
        }

        public async Task UpdateUser(User user)
        {
            await _connection.UpdateAsync(user);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _connection.GetAsync<User>(id);
            return user;
        }

        public async Task<User> GetUser(string email)
        {
            var user = await _connection.QuerySingleOrDefaultAsync<User>(@"select id,email,firstname,lastname,passwordsalt,passwordhash,passwordhashiterations from users where email=@email", new { email }).ConfigureAwait(false);
            return user;
        }
    }
}
