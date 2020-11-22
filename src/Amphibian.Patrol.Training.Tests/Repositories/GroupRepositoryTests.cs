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
    public class GroupRepositoryTests : DatabaseConnectedTestFixture
    {
        private GroupRepository _groupRepository;
        
        [SetUp]
        public void SetUp()
        {
            _groupRepository = new GroupRepository(_connection);
        }

        [Test]
        public async Task TestGetUsersByGroup()
        {
            var result = await _groupRepository.GetUsersInGroup(1);

            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task TestGetGroupsByUser()
        {
            var result = await _groupRepository.GetGroupsForUser(1, 2);

            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task TestGetGroupsBySection()
        {
            var result = await _groupRepository.GetGroupsForSection(1);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task TestGetSectionsUserCanSign()
        {
            var result = await _groupRepository.GetSectionIdsInPlanThatUserCanSign(2, 1);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task TestGetGroupsForUsers()
        {
            var result = await _groupRepository.GetGroupsForUsers(1,new List<int>() { 1, 2 });
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task TestGetGroupsForPatrol()
        {
            var result = await _groupRepository.GetGroupsForPatrol(1);
            Assert.AreEqual(1, result.Count());
        }
    }
}
