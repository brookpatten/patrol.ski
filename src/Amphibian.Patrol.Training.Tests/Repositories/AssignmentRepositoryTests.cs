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
using AutoMapper;
using Amphibian.Patrol.Training.Api.Mappings;

namespace Amphibian.Patrol.Training.Tests.Repositories
{
    class AssignmentRepositoryTests : DatabaseConnectedTestFixture
    {
        private AssignmentRepository _assignmentRepository;
        private PlanRepository _planRepository;
        private UserRepository _userRepository;
        private IMapper _mapper;
        private User _user;
        
        [SetUp]
        public void SetUp()
        {
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _userRepository = new UserRepository(_connection);
            _assignmentRepository = new AssignmentRepository(_connection,_mapper);
            _planRepository = new PlanRepository(_connection,_mapper);
            _user = _userRepository.GetUser(1).Result;
        }

        [Test]
        public async Task CanGetAssignmentsByUser()
        {
            var assignments = await _assignmentRepository.GetAssignmentsForUser(_user.Id);

            Assert.AreEqual(1, assignments.Count());
        }

        [Test]
        public async Task CanGetAssignmentsByPlan()
        {
            var assignments = await _assignmentRepository.GetAssignmentsForPlan(1);

            Assert.AreEqual(1, assignments.Count());
        }

        [Test]
        public async Task CanGetAssignmentsById()
        {
            var assignment = await _assignmentRepository.GetAssignment(1);

            Assert.AreEqual(1, assignment.Id);
        }

        [Test]
        public async Task CanInsertSignature()
        {
            var assignment = await _assignmentRepository.GetAssignment(1);
            var plan = await _planRepository.GetPlan(assignment.PlanId);
            var sections = await _planRepository.GetSectionsForPlan(plan.Id);
            var levels = await _planRepository.GetSectionLevels(sections.First().Id);
            var skills = await _planRepository.GetSectionSkills(sections.First().Id);

            var signature = new Signature()
            {
                AssignmentId = assignment.Id,
                SectionLevelId = levels.First().Id,
                SectionSkillId = skills.First().Id,
                SignedAt = new DateTime(2020, 1, 1,0,0,0),
                SignedByUserId = _user.Id
            };

            await _assignmentRepository.InsertSignature(signature);
        }

        [Test]
        public async Task CanGetSignaturesForAssignment()
        {
            var assignment = await _assignmentRepository.GetAssignment(1);
            var plan = await _planRepository.GetPlan(assignment.PlanId);
            var sections = await _planRepository.GetSectionsForPlan(plan.Id);
            var levels = await _planRepository.GetSectionLevels(sections.First().Id);
            var skills = await _planRepository.GetSectionSkills(sections.First().Id);

            var signature = new Signature()
            {
                AssignmentId = assignment.Id,
                SectionLevelId = levels.First().Id,
                SectionSkillId = skills.First().Id,
                SignedAt = new DateTime(2020, 1, 1, 0, 0, 0),
                SignedByUserId = _user.Id
            };

            await _assignmentRepository.InsertSignature(signature);

            var signatures = await _assignmentRepository.GetSignaturesForAssignment(1);

            Assert.AreEqual(1, signatures.Count());

            Assert.AreEqual(signature.AssignmentId, signatures.First().AssignmentId);
            Assert.AreEqual(signature.SectionLevelId, signatures.First().SectionLevelId);
            Assert.AreEqual(signature.SectionSkillId, signatures.First().SectionSkillId);
            Assert.AreEqual(signature.SignedAt, signatures.First().SignedAt);
            Assert.AreEqual(signature.SignedByUserId, signatures.First().SignedByUserId);
        }

        [Test]
        public async Task CanGetSignaturesWithUsersForAssignment()
        {
            var assignment = await _assignmentRepository.GetAssignment(1);
            var plan = await _planRepository.GetPlan(assignment.PlanId);
            var sections = await _planRepository.GetSectionsForPlan(plan.Id);
            var levels = await _planRepository.GetSectionLevels(sections.First().Id);
            var skills = await _planRepository.GetSectionSkills(sections.First().Id);

            var signature = new Signature()
            {
                AssignmentId = assignment.Id,
                SectionLevelId = levels.First().Id,
                SectionSkillId = skills.First().Id,
                SignedAt = new DateTime(2020, 1, 1, 0, 0, 0),
                SignedByUserId = _user.Id
            };

            await _assignmentRepository.InsertSignature(signature);

            var signatures = await _assignmentRepository.GetSignaturesWithUsersForAssignment(1);

            Assert.AreEqual(1, signatures.Count());

            Assert.AreEqual(signature.SectionLevelId, signatures.First().SectionLevelId);
            Assert.AreEqual(signature.SectionSkillId, signatures.First().SectionSkillId);
            Assert.AreEqual(signature.SignedAt, signatures.First().SignedAt);
            Assert.AreEqual(_user.FirstName, signatures.First().SignedBy.FirstName);
        }
    }
}
