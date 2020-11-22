using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using Dommel;
using System.Linq;

namespace Amphibian.Patrol.Training.Tests.Repositories
{
    public class PatrolRepositoryTests : DatabaseConnectedTestFixture
    {
        private PatrolRepository _patrolRepository;
        private User _user;
        private Amphibian.Patrol.Training.Api.Models.Patrol _patrol;

        [SetUp]
        public void SetUp()
        {
            _patrolRepository = new PatrolRepository(_connection);
            _user = new User()
            {
                Email = "Test@test.com",
                PasswordHash = new byte[32],
                PasswordSalt = new byte[32],
                PasswordHashIterations = 5,
                FirstName = "Test",
                LastName = "Test"
            };
            _user.Id=(int)_connection.Insert(_user);
            _patrol = new Amphibian.Patrol.Training.Api.Models.Patrol()
            {
                Name="Test"
            };
        }

        [Test]
        public async Task CanInsertAndSelectPatrolToDatabase()
        {
            await _patrolRepository.InsertPatrol(_patrol);

            var selected = await _patrolRepository.GetPatrol(_patrol.Id);

            Assert.AreEqual(_patrol.Id, selected.Id);
            Assert.AreEqual(_patrol.Name, selected.Name);
        }

        [Test]
        public async Task CanUpdatePatrolInDatabase()
        {
            await _patrolRepository.InsertPatrol(_patrol);

            _patrol.Name = "Test2";
            await _patrolRepository.UpdatePatrol(_patrol);

            var selected = await _patrolRepository.GetPatrol(_patrol.Id);

            Assert.AreEqual(_patrol.Name, selected.Name);
        }

        [Test]
        public async Task CanInsertPatrolUser()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            var patrolUser = new PatrolUser() { PatrolId = _patrol.Id, UserId = _user.Id };
            await _patrolRepository.InsertPatrolUser(patrolUser);

            var selected = await _patrolRepository.GetPatrolUser(patrolUser.Id);

            Assert.AreEqual(patrolUser.Id, selected.Id);
            Assert.AreEqual(patrolUser.UserId, selected.UserId);
            Assert.AreEqual(patrolUser.PatrolId, selected.PatrolId);
        }

        [Test]
        public async Task CanDeletePatrolUser()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            var patrolUser = new PatrolUser() { PatrolId = _patrol.Id, UserId = _user.Id };
            await _patrolRepository.InsertPatrolUser(patrolUser);

            var selected = await _patrolRepository.GetPatrolUser(patrolUser.Id);
            Assert.NotNull(selected);

            await _patrolRepository.DeletePatrolUser(patrolUser.PatrolId, patrolUser.UserId);

            var selectedAfterDelete = await _patrolRepository.GetPatrolUser(patrolUser.Id);
            Assert.IsNull(selectedAfterDelete);
        }

        [Test]
        public async Task CanGetPatrolUsersForPatrol()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            var patrolUser = new PatrolUser() { PatrolId = _patrol.Id, UserId = _user.Id };
            await _patrolRepository.InsertPatrolUser(patrolUser);

            var allPatrolUsers = await _patrolRepository.GetPatrolUsersForPatrol(_patrol.Id);

            Assert.AreEqual(1, allPatrolUsers.Count());
            Assert.AreEqual(_user.Id, allPatrolUsers.First().UserId);
        }

        [Test]
        public async Task CanGetUsersForPatrol()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            var patrolUser = new PatrolUser() { PatrolId = _patrol.Id, UserId = _user.Id };
            await _patrolRepository.InsertPatrolUser(patrolUser);

            var allPatrolUsers = await _patrolRepository.GetUsersForPatrol(_patrol.Id);

            Assert.AreEqual(1, allPatrolUsers.Count());
            Assert.AreEqual(_user.Id, allPatrolUsers.First().Id);
            Assert.AreEqual(_user.Email, allPatrolUsers.First().Email);
            Assert.AreEqual(_user.FirstName, allPatrolUsers.First().FirstName);
            Assert.AreEqual(_user.LastName, allPatrolUsers.First().LastName);
        }

        [Test]
        public async Task CanGetPatrolUsersForUser()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            var patrolUser = new PatrolUser() { PatrolId = _patrol.Id, UserId = _user.Id };
            await _patrolRepository.InsertPatrolUser(patrolUser);

            var allPatrolUsers = await _patrolRepository.GetPatrolUsersForUser(_user.Id);

            Assert.AreEqual(1, allPatrolUsers.Count());
            Assert.AreEqual(_patrol.Id, allPatrolUsers.First().PatrolId);
        }

        [Test]
        public async Task CanGetPatrolsForUser()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            var patrolUser = new PatrolUser() { PatrolId = _patrol.Id, UserId = _user.Id };
            await _patrolRepository.InsertPatrolUser(patrolUser);

            var allPatrolUsers = await _patrolRepository.GetPatrolsForUser(_user.Id);

            Assert.AreEqual(1, allPatrolUsers.Count());
            Assert.AreEqual(_patrol.Id, allPatrolUsers.First().Id);
            Assert.AreEqual(_patrol.Name, allPatrolUsers.First().Name);
        }
    }
}
