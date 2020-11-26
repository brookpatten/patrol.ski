using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;

namespace Amphibian.Patrol.Tests.Repositories
{
    public class TokenRepositoryTests: DatabaseConnectedTestFixture
    {
        private TokenRepository _tokenRepository;
        private UserRepository _userRepository;
        private User _user;
        private DateTime _createdAt;

        [SetUp]
        public void SetUp()
        {
            _tokenRepository = new TokenRepository(_connection);
            _userRepository = new UserRepository(_connection);
            _user = new User()
            {
                Email = "Test@test.com",
                PasswordHash = new byte[32],
                PasswordSalt = new byte[32],
                PasswordHashIterations = 5,
                FirstName = "Test",
                LastName = "Test"
            };
            _userRepository.InsertUser(_user).Wait();
            _createdAt = new DateTime(2020, 1, 1,0,0,0,DateTimeKind.Utc);
        }

        [Test]
        public async Task CanInsertAndSelectUserTokenGuidToDatabase()
        {
            var tokenGuid = Guid.NewGuid();
            var token = new Token();
            token.TokenGuid = tokenGuid;
            token.CreatedAt = _createdAt;
            token.LastRequestAt = _createdAt;
            token.UserId = _user.Id;

            await _tokenRepository.InsertToken(token);

            var token2 = await _tokenRepository.GetToken(tokenGuid);

            Assert.AreEqual(token.TokenGuid, token2.TokenGuid);
        }

        [Test]
        public async Task CanQueryUserIdFromDatabase()
        {
            var tokenGuid = Guid.NewGuid();
            var token = new Token();
            token.TokenGuid = tokenGuid;
            token.CreatedAt = _createdAt;
            token.LastRequestAt = _createdAt;
            token.UserId = _user.Id;

            await _tokenRepository.InsertToken(token);

            var token2 = await _tokenRepository.GetToken(token.Id);

            Assert.AreEqual(token.UserId, token2.UserId);
        }

        [Test]
        public async Task CanQuerCreatedAtFromDatabase()
        {
            var tokenGuid = Guid.NewGuid();
            var token = new Token();
            token.TokenGuid = tokenGuid;
            token.CreatedAt = _createdAt;
            token.LastRequestAt = _createdAt;
            token.UserId = _user.Id;

            await _tokenRepository.InsertToken(token);

            var token2 = await _tokenRepository.GetToken(token.Id);

            Assert.AreEqual(token.CreatedAt, token2.CreatedAt);
        }

        [Test]
        public async Task CanQuerLastRequestAtFromDatabase()
        {
            var tokenGuid = Guid.NewGuid();
            var token = new Token();
            token.TokenGuid = tokenGuid;
            token.CreatedAt = _createdAt;
            token.LastRequestAt = _createdAt;
            token.UserId = _user.Id;

            await _tokenRepository.InsertToken(token);

            var token2 = await _tokenRepository.GetToken(token.Id);

            Assert.AreEqual(token.LastRequestAt, token2.LastRequestAt);
        }
    }
}
