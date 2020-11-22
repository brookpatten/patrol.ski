using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Repositories;
using Dommel;
using System.Linq;
using AutoMapper;
using Amphibian.Patrol.Training.Api.Mappings;
using Amphibian.Patrol.Training.Api.Dtos;

namespace Amphibian.Patrol.Training.Tests.Repositories
{
    public class PlanRepositoryTests : DatabaseConnectedTestFixture
    {
        private PlanRepository _planRepository;
        private PatrolRepository _patrolRepository;
        private UserRepository _userRepository;
        private User _user;
        private CurrentUserPatrolDto _patrol;
        private Plan _plan;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _planRepository = new PlanRepository(_connection,_mapper);
            _patrolRepository = new PatrolRepository(_connection);
            _userRepository = new UserRepository(_connection);

            _user = _userRepository.GetUser(1).Result;
            _patrol = _patrolRepository.GetPatrolsForUser(_user.Id).Result.First(x => x.Name == "Big Mountain Patrol");
            _plan = _planRepository.GetPlansForPatrol(_patrol.Id).Result.First(x => x.Name == "Ski Alpine");
        }

        [Test]
        public async Task CanInsertAndSelectPlanToDatabase()
        {
            var plan = new Plan() { Name = "Test", PatrolId = _patrol.Id };
            await _planRepository.InsertPlan(plan);

            var selected = await _planRepository.GetPlan(plan.Id);

            Assert.AreEqual(plan.Id, selected.Id);
            Assert.AreEqual(plan.Name, selected.Name);
        }

        [Test]
        public async Task CanUpdatePlanInDatabase()
        {
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
            var plans = await _planRepository.GetPlansForPatrol(_patrol.Id);

            Assert.AreEqual(2, plans.Count());
        }

        [Test]
        public async Task CanGetSectionsForPlan()
        {
            var sections = await _planRepository.GetSectionsForPlan(_plan.Id);

            Assert.AreEqual(3, sections.Count());
        }

        [Test]
        public async Task CanGetSkillsForSection()
        {
            var sections = await _planRepository.GetSectionsForPlan(_plan.Id);
            var section = sections.OrderBy(x=>x.Id).First();
            var skills = await _planRepository.GetSectionSkills(section.Id);

            Assert.AreEqual(8, skills.Count());
        }

        [Test]
        public async Task CanGetSectionSkillsForPlan()
        {
            var skills = await _planRepository.GetSectionSkillsForPlan(_plan.Id);

            Assert.AreEqual(22, skills.Count());
        }

        [Test]
        public async Task CanGetLevelsForSection()
        {
            var sections = await _planRepository.GetSectionsForPlan(_plan.Id);
            var section = sections.OrderBy(x => x.Id).First();
            var skills = await _planRepository.GetSectionLevels(section.Id);

            Assert.AreEqual(3, skills.Count());
        }

        [Test]
        public async Task CanGetSectionLevelsForPlan()
        {
            var skills = await _planRepository.GetSectionLevelsForPlan(_plan.Id);

            Assert.AreEqual(7, skills.Count());
        }

        [Test]
        public async Task CanGetLevelsForPatrol()
        {
            var levels = await _planRepository.GetLevels(_patrol.Id);

            Assert.AreEqual(3, levels.Count());
        }

        [Test]
        public async Task CanGetSkillsForPatrol()
        {
            var skills = await _planRepository.GetSkills(_patrol.Id);

            Assert.AreEqual(19, skills.Count());
        }
    }
}
