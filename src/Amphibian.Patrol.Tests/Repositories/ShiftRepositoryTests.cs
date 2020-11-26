using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Dommel;
using System.Linq;
using Amphibian.Patrol.Api.Mappings;

namespace Amphibian.Patrol.Tests.Repositories
{
    public class ShiftRepositoryTests : DatabaseConnectedTestFixture
    {
        private ShiftRepository _shiftRepository;
        private AssignmentRepository _assignmentRepository;
        private PlanRepository _planRepository;
        
        [SetUp]
        public void SetUp()
        {
            var mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _shiftRepository = new ShiftRepository(_connection);
            _assignmentRepository = new AssignmentRepository(_connection,mapper);
            _planRepository = new PlanRepository(_connection, mapper);
        }

        [Test]
        public async Task CanRetrieveRelevantTrainingShiftsForTrainee()
        {
            var traineeUserId = 1;
            var patrolId = 1;
            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            Assert.AreEqual(1, availableShifts.Count());
        }

        [Test]
        public async Task NoTrainingShiftsAreReturnedIfNoIncompleteAssignments()
        {
            var traineeUserId = 1;
            var patrolId = 1;

            var assignments = await _assignmentRepository.GetAssignmentsForUser(traineeUserId);
            foreach(var assignmentDto in assignments)
            {
                var assignment = await _assignmentRepository.GetAssignment(assignmentDto.Id);
                assignment.CompletedAt = DateTime.Now;
                await _assignmentRepository.UpdateAssignment(assignment);
            }

            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            Assert.AreEqual(0, availableShifts.Count());
        }

        [Test]
        public async Task NoTrainingShiftsAreReturnedIfNoMissingSignatures()
        {
            var trainerUserId = 2;
            var traineeUserId = 1;
            var patrolId = 1;

            var assignments = await _assignmentRepository.GetAssignmentsForUser(traineeUserId);
            foreach (var assignmentDto in assignments)
            {
                var assignment = await _assignmentRepository.GetAssignment(assignmentDto.Id);

                var sections = await _planRepository.GetSectionsForPlan(assignment.PlanId);
                var signatures = await _assignmentRepository.GetSignaturesForAssignment(assignment.Id);

                foreach(var section in sections)
                {
                    var sectionlevels = await _planRepository.GetSectionLevels(section.Id);
                    var sectionSkills = await _planRepository.GetSectionSkills(section.Id);

                    foreach(var sectionSkill in sectionSkills)
                    {
                        foreach(var sectionlevel in sectionlevels)
                        {
                            var existingSig = signatures.FirstOrDefault(x => x.SectionLevelId == sectionlevel.Id && x.SectionSkillId == sectionSkill.Id);
                            if(existingSig==null)
                            {
                                var sig = new Signature()
                                {
                                    AssignmentId = assignment.Id,
                                    SectionLevelId = sectionlevel.Id,
                                    SectionSkillId = sectionSkill.Id,
                                    SignedAt = DateTime.Now,
                                    SignedByUserId = trainerUserId
                                };
                                await _assignmentRepository.InsertSignature(sig);
                            }

                        }
                    }
                }
            }

            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            Assert.AreEqual(0, availableShifts.Count());
        }

        [Test]
        public async Task NoTrainingShiftsReturnedIfAlreadyCommitted()
        {
            var traineeUserId = 1;
            var patrolId = 1;
            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            
            foreach(var available in availableShifts)
            {
                var trainee = new Trainee()
                {
                    ShiftTrainerId = available.Id,
                    TraineeUserId = traineeUserId
                };
                await _shiftRepository.InsertTrainee(trainee);
            }

            var noAvailableShiftsRemain = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            Assert.AreEqual(0, noAvailableShiftsRemain.Count());

        }

        [Test]
        public async Task CanRetrieveCommittedShiftsWhenNone()
        {
            var traineeUserId = 1;
            var patrolId = 1;
            var availableShifts = await _shiftRepository.GetCommittedTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);

            Assert.AreEqual(0, availableShifts.Count());
        }

        [Test]
        public async Task CanRetrieveCommittedShifts()
        {
            var traineeUserId = 1;
            var patrolId = 1;
            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);

            int count = 0;
            foreach (var available in availableShifts)
            {
                var trainee = new Trainee()
                {
                    ShiftTrainerId = available.Id,
                    TraineeUserId = traineeUserId
                };
                await _shiftRepository.InsertTrainee(trainee);
                count++;
            }

            var committed = await _shiftRepository.GetCommittedTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            Assert.AreEqual(count, committed.Count());
        }

        [Test]
        public async Task CanGetTrainingShiftsForTrainer()
        {
            var trainerUserId = 2;
            var patrolId = 1;

            var shifts = await _shiftRepository.GetTrainerShifts(patrolId, trainerUserId, DateTime.Now);

            Assert.AreEqual(1, shifts.Count());
        }
    }
}
