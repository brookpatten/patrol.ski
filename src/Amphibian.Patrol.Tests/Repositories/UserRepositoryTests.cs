using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using System.Linq;

namespace Amphibian.Patrol.Tests.Repositories
{
    public class UserRepositoryTests: DatabaseConnectedTestFixture
    {
        private UserRepository _userRepository;

        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(_connection);
        }

        [Test]
        public async Task CanInsertAndSelectUserEmailToDatabase()
        {
            var user = new User();
            user.Email = "A";
            user.FirstName = "B";
            user.LastName = "C";
            user.PasswordHash = new byte[] { 1 };
            user.PasswordSalt = new byte[] { 2 };
            user.PasswordHashIterations = 1;

            await _userRepository.InsertUser(user);

            var user2 = await _userRepository.GetUser(user.Id);

            Assert.AreEqual(user.Email, user2.Email);
        }

        [Test]
        public async Task CanInsertAndSelectUserFirstNameToDatabase()
        {
            var user = new User();
            user.Email = "A";
            user.FirstName = "B";
            user.LastName = "C";
            user.PasswordHash = new byte[] { 1 };
            user.PasswordSalt = new byte[] { 2 };
            user.PasswordHashIterations = 1;

            await _userRepository.InsertUser(user);

            var user2 = await _userRepository.GetUser(user.Id);

            Assert.AreEqual(user.FirstName, user2.FirstName);
        }

        [Test]
        public async Task CanGetUsersByIds()
        {
            var users = await _userRepository.GetUsers(new List<int>() { 1, 2, 3 });

            Assert.AreEqual(3,users.Count());
        }
    }
}
