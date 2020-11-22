using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dommel;

using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Amphibian.Patrol.Training.Api.Extensions;
using Amphibian.Patrol.Training.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Infrastructure;
using Amphibian.Patrol.Training.Tests.Repositories;
using NUnit.Framework;

namespace Amphibian.Patrol.Training.Tests.Infrastructure
{
    public class UnitOfWorkTests: DatabaseConnectedTestFixture
    {
        private DbUnitOfWork _unitOfWork;
        
        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new DbUnitOfWork(_connection);
        }

        [Test]
        public async Task CanCommitChanges()
        {
            await _unitOfWork.Begin();

            var id = (int)await _connection.InsertAsync(new Token()
            {
                TokenGuid = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                LastRequestAt = DateTime.Now,
                UserId = 1
            });

            await _unitOfWork.Commit();

            var token = await _connection.GetAsync<Token>(id);

            Assert.NotNull(token);
        }

        [Test]
        public async Task CanRollbackChanges()
        {
            await _unitOfWork.Begin();

            var id = (int)await _connection.InsertAsync(new Token()
            {
                TokenGuid = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                LastRequestAt = DateTime.Now,
                UserId = 1
            });

            await _unitOfWork.Rollback();

            var differentConnection = this.CreateAdditionalConnection();
            
            var token = await differentConnection.GetAsync<Token>(id);

            Assert.Null(token);
        }

        [Test]
        public async Task CanCommitChangesOnPreviouslyOpenedConnection()
        {
            await _connection.OpenAsync();
            await _unitOfWork.Begin();

            var id = (int)await _connection.InsertAsync(new Token()
            {
                TokenGuid = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                LastRequestAt = DateTime.Now,
                UserId = 1
            });

            await _unitOfWork.Commit();

            var differentConnection = this.CreateAdditionalConnection();

            var token = await differentConnection.GetAsync<Token>(id);

            Assert.NotNull(token);
        }

        [Test]
        public async Task CanRollbackChangesOnPreviouslyOpenedConnection()
        {
            await _connection.OpenAsync();
            await _unitOfWork.Begin();

            var id = (int)await _connection.InsertAsync(new Token()
            {
                TokenGuid = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                LastRequestAt = DateTime.Now,
                UserId = 1
            });

            await _unitOfWork.Rollback();

            var differentConnection = this.CreateAdditionalConnection();

            var token = await differentConnection.GetAsync<Token>(id);

            Assert.Null(token);
        }
    }
}
