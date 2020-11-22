using Amphibian.Patrol.Training.Api.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers(IList<int> ids);
        Task<User> GetUser(string email);
        Task InsertUser(User user);
        Task UpdateUser(User user);
    }
}