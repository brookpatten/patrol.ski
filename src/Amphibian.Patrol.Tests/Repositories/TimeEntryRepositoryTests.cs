using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Dommel;
using System.Linq;
using System.Runtime.InteropServices;
using AutoMapper;
using Amphibian.Patrol.Api.Mappings;

namespace Amphibian.Patrol.Tests.Repositories
{
    public class TimeEntryRepositoryTests : DatabaseConnectedTestFixture
    {
        private ShiftRepository _shiftRepository;
        private TimeEntryRepository _timeEntryRepository;
        private IMapper _mapper;
        private User _user;

        [SetUp]
        public void SetUp()
        {
            _timeEntryRepository = new TimeEntryRepository(this._connection);
            _shiftRepository = new ShiftRepository(this._connection);
        }

        [Test]
        public async Task CanInsertTimeEntry()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };

            await _timeEntryRepository.InsertTimeEntry(entry);

            Assert.AreEqual(1, entry.Id);
        }

        [Test]
        public async Task CanUpdateTimeEntry()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };

            await _timeEntryRepository.InsertTimeEntry(entry);

            entry.ClockOut = now;

            await _timeEntryRepository.UpdateTimeEntry(entry);

            var after = await _timeEntryRepository.GetTimeEntry(entry.Id);

            Assert.AreEqual(now, entry.ClockOut);
        }

        [Test]
        public async Task CanDeleteTimeEntry()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };

            await _timeEntryRepository.InsertTimeEntry(entry);

            await _timeEntryRepository.DeleteTimeEntry(entry);

            var after = await _timeEntryRepository.GetTimeEntry(entry.Id);

            Assert.IsNull(after);
        }

        [Test]
        public async Task CanGetActiveTimeEntriesByUser()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            });

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId+1,
                UserId = userId
            });

            var entries = await _timeEntryRepository.GetActiveTimeEntries(null, userId);

            Assert.AreEqual(2, entries.Count());
        }

        [Test]
        public async Task CanGetActiveTimeEntriesByUserAndPatrol()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            });

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId + 1,
                UserId = userId
            });

            var entries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, userId);

            Assert.AreEqual(1, entries.Count());
        }

        [Test]
        public async Task CanGetActiveTimeEntriesByPatrol()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId+1
            });

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            });

            var entries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, null);

            Assert.AreEqual(2, entries.Count());
        }

        [Test]
        public async Task CanInsertTimeEntryScheduledShiftAssignment()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };
            await _timeEntryRepository.InsertTimeEntry(entry);

            var scheduledShift = new ScheduledShift()
            {
                StartsAt = now,
                EndsAt = now + new TimeSpan(1, 0, 0),
                DurationSeconds = (int)(new TimeSpan(1, 0, 0)).TotalSeconds,
                PatrolId = patrolId,
            };
            await _shiftRepository.InsertScheduledShift(scheduledShift);

            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                ScheduledShiftId = scheduledShift.Id,
                Status = ShiftStatus.Assigned
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
            {
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                TimeEntryId = entry.Id
            };
            await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);


            Assert.AreEqual(1, timeEntryScheduledShiftAssignment.Id);
        }

        [Test]
        public async Task CanUpdateTimeEntryScheduledShiftAssignment()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };
            await _timeEntryRepository.InsertTimeEntry(entry);

            var scheduledShift = new ScheduledShift()
            {
                StartsAt = now,
                EndsAt = now + new TimeSpan(1, 0, 0),
                DurationSeconds = (int)(new TimeSpan(1, 0, 0)).TotalSeconds,
                PatrolId = patrolId,
            };
            await _shiftRepository.InsertScheduledShift(scheduledShift);

            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                ScheduledShiftId = scheduledShift.Id,
                Status = ShiftStatus.Assigned
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
            {
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                TimeEntryId = entry.Id
            };
            await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);

            timeEntryScheduledShiftAssignment.DurationSeconds = 1000;
            await _timeEntryRepository.UpdateTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);

            var after = await _timeEntryRepository.GetTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment.Id);


            Assert.AreEqual(1000, after.DurationSeconds);
        }

        [Test]
        public async Task CanDeleteTimeEntryScheduledShiftAssignment()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };
            await _timeEntryRepository.InsertTimeEntry(entry);

            var scheduledShift = new ScheduledShift()
            {
                StartsAt = now,
                EndsAt = now + new TimeSpan(1, 0, 0),
                DurationSeconds = (int)(new TimeSpan(1, 0, 0)).TotalSeconds,
                PatrolId = patrolId,
            };
            await _shiftRepository.InsertScheduledShift(scheduledShift);

            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                ScheduledShiftId = scheduledShift.Id,
                Status = ShiftStatus.Assigned
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
            {
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                TimeEntryId = entry.Id
            };
            await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);

            await _timeEntryRepository.DeleteTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);

            var after = await _timeEntryRepository.GetTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment.Id);


            Assert.IsNull(after);
        }

        [Test]
        public async Task CanGetTimeEntryScheduledShiftAssignmentsByTimeEntry()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };
            await _timeEntryRepository.InsertTimeEntry(entry);

            var scheduledShift = new ScheduledShift()
            {
                StartsAt = now,
                EndsAt = now + new TimeSpan(1, 0, 0),
                DurationSeconds = (int)(new TimeSpan(1, 0, 0)).TotalSeconds,
                PatrolId = patrolId,
            };
            await _shiftRepository.InsertScheduledShift(scheduledShift);

            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                ScheduledShiftId = scheduledShift.Id,
                Status = ShiftStatus.Assigned
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
            {
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                TimeEntryId = entry.Id
            };
            await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);

            var entries = await _timeEntryRepository.GetScheduledShiftAssignmentsForTimeEntry(entry.Id);


            Assert.AreEqual(1, entries.Count());
        }

        [Test]
        public async Task CanGetTimeEntryScheduledShiftAssignmentsByScheduledShiftAssignmentId()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var entry = new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId
            };
            await _timeEntryRepository.InsertTimeEntry(entry);

            var scheduledShift = new ScheduledShift()
            {
                StartsAt = now,
                EndsAt = now + new TimeSpan(1, 0, 0),
                DurationSeconds = (int)(new TimeSpan(1, 0, 0)).TotalSeconds,
                PatrolId = patrolId,
            };
            await _shiftRepository.InsertScheduledShift(scheduledShift);

            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                ScheduledShiftId = scheduledShift.Id,
                Status = ShiftStatus.Assigned
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
            {
                ScheduledShiftAssignmentId = scheduledShiftAssignment.Id,
                TimeEntryId = entry.Id
            };
            await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);

            var entries = await _timeEntryRepository.GetScheduledShiftAssignmentsForScheduledShiftAssignment(scheduledShiftAssignment.Id);


            Assert.AreEqual(1, entries.Count());
        }

        [Test]
        public async Task CanGetActiveTimeEntries()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now - new TimeSpan(1, 0, 0, 0),
                ClockOut = now - new TimeSpan(0,1,0,0),
                PatrolId = patrolId,
                UserId = userId
            }); ; ;

            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now,
                PatrolId = patrolId,
                UserId = userId +1
            });

            var entries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, now);


            Assert.AreEqual(1, entries.Count());
        }

        [Test]
        public async Task CanGetMissingTimeEntries()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            var scheduledShift = new ScheduledShift()
            {
                StartsAt = now - new TimeSpan(1,0,0),
                EndsAt = now + new TimeSpan(1, 0, 0),
                DurationSeconds = (int)(new TimeSpan(2, 0, 0)).TotalSeconds,
                PatrolId = patrolId,
            };
            await _shiftRepository.InsertScheduledShift(scheduledShift);

            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                ScheduledShiftId = scheduledShift.Id,
                Status = ShiftStatus.Assigned
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            
            var entries = await _timeEntryRepository.GetMissingTimeEntries(patrolId,now);


            Assert.AreEqual(1, entries.Count());
        }

        [Test]
        public async Task CanGetTimeEntries()
        {
            var now = new DateTime(2001, 1, 1);
            int userId = 1;
            int patrolId = 1;

            //before
            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now - new TimeSpan(1, 0, 0, 0),
                ClockOut = now + new TimeSpan(0, 1, 0, 0),
                PatrolId = patrolId,
                UserId = userId
            }); ; ;

            //during
            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now+new TimeSpan(0,1,0),
                ClockOut = now +new TimeSpan(0,2,0),
                PatrolId = patrolId,
                UserId = userId
            });

            //after
            await _timeEntryRepository.InsertTimeEntry(new TimeEntry()
            {
                ClockIn = now + new TimeSpan(0, 1, 0),
                ClockOut = now + new TimeSpan(10, 0, 0),
                PatrolId = patrolId,
                UserId = userId
            });

            var entries = await _timeEntryRepository.GetTimeEntries(patrolId, userId,now,now+new TimeSpan(5,0,0));


            Assert.AreEqual(3, entries.Count());
        }
    }
}
