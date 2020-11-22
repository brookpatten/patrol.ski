using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using Dapper.Contrib.Extensions;
using System.Linq;

namespace Amphibian.Patrol.Training.Tests.Repositories
{
    public class PlanRepositoryTests : DatabaseConnectedTestFixture
    {
        private PlanRepository _planRepository;
        private PatrolRepository _patrolRepository;
        private Amphibian.Patrol.Training.Api.Models.Patrol _patrol;
        private Plan _plan;

        [SetUp]
        public void SetUp()
        {
            _planRepository = new PlanRepository(_connection);
            _patrolRepository = new PatrolRepository(_connection);
            _patrol = new Amphibian.Patrol.Training.Api.Models.Patrol()
            {
                Name = "Test"
            };
            _plan = new Plan()
            {
                Name="Test"
            };
        }

        [Test]
        public async Task CanInsertAndSelectPlanToDatabase()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            _plan.PatrolId = _patrol.Id;
            await _planRepository.InsertPlan(_plan);

            var selected = await _planRepository.GetPlan(_plan.Id);

            Assert.AreEqual(_plan.Id, selected.Id);
            Assert.AreEqual(_plan.Name, selected.Name);
        }

        [Test]
        public async Task CanUpdatePlanInDatabase()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            _plan.PatrolId = _patrol.Id;
            await _planRepository.InsertPlan(_plan);

            var selected = await _planRepository.GetPlan(_plan.Id);

            Assert.AreEqual(_plan.Name, selected.Name);

            _plan.Name = "Test 2";
            await _planRepository.UpdatePlan(_plan);

            var selected2 = await _planRepository.GetPlan(_plan.Id);

            Assert.AreEqual(_plan.Name, selected2.Name);
        }

        [Test]
        public async Task CanGetPlansForPatrol()
        {
            await _patrolRepository.InsertPatrol(_patrol);
            _plan.PatrolId = _patrol.Id;
            await _planRepository.InsertPlan(_plan);

            var plans = await _planRepository.GetPlansForPatrol(_patrol.Id);

            Assert.AreEqual(1, plans.Count());
            Assert.AreEqual(_plan.Id, plans.First().Id);
            Assert.AreEqual(_plan.Name, plans.First().Name);
        }
    }
}
