using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data;
using Dapper;
using Dommel;

using Amphibian.Patrol.Training.Api.Models;
using System.Data.Common;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnection _connection;

        public UserRepository(DbConnection connection)
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

        public async Task<IEnumerable<User>> GetUsers(IList<int> userIds)
        {
            var users = await _connection.QueryAsync<User>("select * from users where id in @userIds", new { userIds });
            return users;
        }

        public async Task<User> GetUser(string email)
        {
            var users = await _connection.SelectAsync<User>(x => x.Email == email);
            if(users.Any())
            {
                return users.First();
            }
            else
            {
                return null;
            }
        }
    }
}
