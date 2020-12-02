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

        //training related tests
        [Test]
        public async Task CanRetrieveRelevantTrainingShiftsForTrainee()
        {
            var traineeUserId = 1;
            var patrolId = 1;
            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, traineeUserId, new DateTime(2001, 1, 1));
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
                    ScheduledShiftAssignmentId = available.Id,
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
                    ScheduledShiftAssignmentId = available.Id,
                    TraineeUserId = traineeUserId
                };
                await _shiftRepository.InsertTrainee(trainee);
                count++;
            }

            var committed = await _shiftRepository.GetCommittedTrainerShiftsForTrainee(patrolId, traineeUserId, DateTime.Now);
            Assert.AreEqual(count, committed.Count());
        }

        [Test]
        public async Task CanGetScheduledShiftAssignmentsForUser()
        {
            var trainerUserId = 1;
            var patrolId = 1;

            var shifts = await _shiftRepository.GetScheduledShiftAssignments(patrolId, trainerUserId, new DateTime(2001,1,1));
            
            Assert.AreEqual(1, shifts.Count());
        }

        [Test]
        public async Task CanGetScheduledShiftAssignmentsByDateRange()
        {
            var userId = 1;
            var patrolId = 1;

            var shifts = await _shiftRepository.GetScheduledShiftAssignments(patrolId, null, new DateTime(2000,12,31), new DateTime(2001,1,2));

            Assert.AreEqual(3, shifts.Count());
        }

        [Test]
        public async Task CanGetScheduledShiftAssignmentsByStatus()
        {
            var trainerUserId = 1;
            var patrolId = 1;

            var shifts = await _shiftRepository.GetScheduledShiftAssignments(patrolId, status: ShiftStatus.Released);

            Assert.AreEqual(1, shifts.Count());
        }

        [Test]
        public async Task CanGetScheduledShiftById()
        {
            var shift = await _shiftRepository.GetScheduledShift(1);

            Assert.NotNull(shift);
        }

        [Test]
        public async Task CanUpdateScheduledShift()
        {
            var before = await _shiftRepository.GetScheduledShift(1);
            var change = await _shiftRepository.GetScheduledShift(1);
            change.EndsAt = new DateTime(2001, 1, 1, 8, 0, 0);
            await _shiftRepository.UpdateScheduledShift(change);
            var after = await _shiftRepository.GetScheduledShift(1);

            Assert.AreEqual(after.EndsAt, change.EndsAt);
        }

        [Test]
        public async Task CanInsertScheduledShift()
        {
            var before = new ScheduledShift()
            {
                StartsAt = new DateTime(2001, 1, 1, 8, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 14, 0, 0),
                GroupId = null,
                PatrolId = 1,
                ShiftId = null
            };

            await _shiftRepository.InsertScheduledShift(before);

            var after = await _shiftRepository.GetScheduledShift(before.Id);

            Assert.AreEqual(before.StartsAt,after.StartsAt);
            Assert.AreEqual(before.EndsAt, after.EndsAt);
            Assert.AreEqual(before.GroupId, after.GroupId);
            Assert.AreEqual(before.PatrolId, after.PatrolId);
            Assert.AreEqual(before.ShiftId, after.ShiftId);
        }

        [Test]
        public async Task CanDeleteScheduledShift()
        {
            var before = new ScheduledShift()
            {
                StartsAt = new DateTime(2001, 1, 1, 8, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 14, 0, 0),
                GroupId = null,
                PatrolId = 1,
                ShiftId = null
            };

            await _shiftRepository.InsertScheduledShift(before);

            await _shiftRepository.DeleteScheduledShift(before);

            var after = await _shiftRepository.GetScheduledShift(before.Id);

            Assert.IsNull(after);
        }

        [Test]
        public async Task CanGetShiftsForPatrol()
        {
            var shifts = await _shiftRepository.GetShifts(1);

            Assert.AreEqual(2, shifts.Count());
        }

        [Test]
        public async Task CanGetShiftsByTime()
        {
            var shifts = await _shiftRepository.GetShifts(1,9,0,13,0);

            Assert.AreEqual(1, shifts.Count());
        }

        [Test]
        public async Task CanGetShiftById()
        {
            var shifts = await _shiftRepository.GetShifts(1);
            var shift = await _shiftRepository.GetShift(shifts.First().Id);

            Assert.NotNull(shift);
        }

        [Test]
        public async Task CanInsertShift()
        {
            var before = new Shift()
            {
                PatrolId = 1,
                Name="Test",
                StartHour = 1,
                StartMinute = 2,
                EndHour = 3,
                EndMinute =4
            };

            await _shiftRepository.InsertShift(before);

            var after = await _shiftRepository.GetShift(before.Id);

            Assert.AreEqual(before.Name, after.Name);
            Assert.AreEqual(before.StartHour, after.StartHour);
            Assert.AreEqual(before.StartMinute, after.StartMinute);
            Assert.AreEqual(before.PatrolId, after.PatrolId);
            Assert.AreEqual(before.EndHour, after.EndHour);
            Assert.AreEqual(before.EndMinute, after.EndMinute);
        }

        [Test]
        public async Task CanUpdateShift()
        {
            var before = new Shift()
            {
                PatrolId = 1,
                Name = "Test",
                StartHour = 1,
                StartMinute = 2,
                EndHour = 3,
                EndMinute = 4
            };

            await _shiftRepository.InsertShift(before);

            before.Name = "Test2";
            before.StartHour = 2;
            before.StartMinute = 3;
            before.EndHour = 4;
            before.EndMinute = 5;

            await _shiftRepository.UpdateShift(before);



            var after = await _shiftRepository.GetShift(before.Id);

            Assert.AreEqual(before.Name, after.Name);
            Assert.AreEqual(before.StartHour, after.StartHour);
            Assert.AreEqual(before.StartMinute, after.StartMinute);
            Assert.AreEqual(before.PatrolId, after.PatrolId);
            Assert.AreEqual(before.EndHour, after.EndHour);
            Assert.AreEqual(before.EndMinute, after.EndMinute);
        }

        [Test]
        public async Task CanDeleteShift()
        {
            var before = new Shift()
            {
                PatrolId = 1,
                Name = "Test",
                StartHour = 1,
                StartMinute = 2,
                EndHour = 3,
                EndMinute = 4
            };

            await _shiftRepository.InsertShift(before);
            await _shiftRepository.DeleteShift(before);

            var after = await _shiftRepository.GetShift(before.Id);

            Assert.IsNull(after);
        }

        [Test]
        public async Task CanInsertScheduledShiftAssignment()
        {
            var before = new ScheduledShiftAssignment()
            {
                AssignedUserId = 1,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned,
            };

            await _shiftRepository.InsertScheduledShiftAssignment(before);

            var after = await _shiftRepository.GetScheduledShiftAssignment(before.Id);

            Assert.AreEqual(before.AssignedUserId, after.AssignedUserId);
            Assert.AreEqual(before.OriginalAssignedUserId, after.OriginalAssignedUserId);
            Assert.AreEqual(before.ScheduledShiftId, after.ScheduledShiftId);
            Assert.AreEqual(before.Status, after.Status);
        }

        [Test]
        public async Task CanUpdateScheduledShiftAssignment()
        {
            var before = new ScheduledShiftAssignment()
            {
                AssignedUserId = 1,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned,
            };

            await _shiftRepository.InsertScheduledShiftAssignment(before);

            before.Status = ShiftStatus.Released;

            await _shiftRepository.UpdateScheduledShiftAssignment(before);

            var after = await _shiftRepository.GetScheduledShiftAssignment(before.Id);

            Assert.AreEqual(before.Status, after.Status);
        }

        [Test]
        public async Task CanDeleteScheduledShiftAssignment()
        {
            var before = new ScheduledShiftAssignment()
            {
                AssignedUserId = 1,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned,
            };

            await _shiftRepository.InsertScheduledShiftAssignment(before);
            await _shiftRepository.DeleteScheduledShiftAssignment(before);

            var after = await _shiftRepository.GetScheduledShiftAssignment(before.Id);

            Assert.IsNull(after);
        }
        [Test]
        public async Task CanGetScheduledShiftAssignmentsByScheduledShiftId()
        {
            var after = await _shiftRepository.GetScheduledShiftAssignments(1);

            Assert.AreEqual(3, after.Count());
        }
    }
}
