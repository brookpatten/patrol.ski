using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using Dapper.Contrib.Extensions;
using System.Linq;
using System.Runtime.InteropServices;

namespace Amphibian.Patrol.Training.Tests.Repositories
{
    class AssignmentRepositoryTests : DatabaseConnectedTestFixture
    {
        private AssignmentRepository _assignmentRepository;
        private UserRepository _userRepository;
        private User _user;

        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(_connection);
            _assignmentRepository = new AssignmentRepository(_connection);
            _user = _userRepository.GetUser(1).Result;
        }

        [Test]
        public async Task CanGetAssignmentsByUser()
        {
            var assignments = await _assignmentRepository.GetAssignmentsForUser(_user.Id);

            Assert.AreEqual(1, assignments.Count());
        }

        [Test]
        public async Task CanGetAssignmentsById()
        {
            var assignment = await _assignmentRepository.GetAssignment(1);

            Assert.AreEqual(1, assignment.Id);
        }
    }
}
