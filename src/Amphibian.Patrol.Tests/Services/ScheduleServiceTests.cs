using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Dtos;
using AutoMapper;
using Moq;
using Amphibian.Patrol.Api.Mappings;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Tests.Services
{
    [TestFixture(Category = "Services")]
    public class ScheduleServiceTests
    {
        private ScheduleService _scheduleService;
        private Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<ScheduleService>> _loggerMock;
        private Mock<IShiftRepository> _shiftRepositoryMock;
        private Mock<IGroupRepository> _groupRepositoryMock;
        private Mock<IPatrolRepository> _patrolRepository;
        private Mock<ISystemClock> _systemClockMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void Setup()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _loggerMock = new Mock<ILogger<ScheduleService>>();
            _shiftRepositoryMock = new Mock<IShiftRepository>();
            _patrolRepository = new Mock<IPatrolRepository>();
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _systemClockMock = new Mock<ISystemClock>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            _scheduleService = new ScheduleService(_loggerMock.Object, _patrolRepository.Object, _groupRepositoryMock.Object, _shiftRepositoryMock.Object, _systemClockMock.Object,_emailServiceMock.Object,_userRepositoryMock.Object);
        }

        [Test]
        public async Task CanReleaseShift()
        {
            int id = 1;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id=1,
                AssignedUserId=1,
                ClaimedByUserId=null,
                OriginalAssignedUserId=1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.UpdateScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Status == ShiftStatus.Released && y.ClaimedByUserId == null))).Verifiable();

            await _scheduleService.ReleaseShift(id);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task ReleaseShiftInClaimedStateThrows()
        {
            int id = 1;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = null,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Claimed
            })).Verifiable();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _scheduleService.ReleaseShift(id);
            });

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanClaimReleasedShift()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = null,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Released
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShift(id)).Returns(Task.FromResult(new ScheduledShift()
            {
                Id = 1,
                StartsAt = DateTime.Now,
                EndsAt = DateTime.Now
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.UpdateScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Status == ShiftStatus.Claimed && y.ClaimedByUserId == userId))).Verifiable();

            await _scheduleService.ClaimShift(id, userId);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task ClaimShiftInInvalidStateThrows()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = null,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Claimed
            })).Verifiable();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _scheduleService.ClaimShift(id,userId);
            });

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanApproveClaimedShift()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = userId,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Claimed
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShift(1)).Returns(Task.FromResult(new ScheduledShift()
            {
                Id = 1,
                StartsAt = DateTime.Now,
                EndsAt = DateTime.Now
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.UpdateScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Status == ShiftStatus.Assigned 
            && y.ClaimedByUserId == null
            && y.AssignedUserId == userId
            && y.OriginalAssignedUserId ==1))).Verifiable();

            await _scheduleService.ApproveShiftSwap(id,1);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task ApproveShiftClaimInInvalidStateThrows()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = null,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned
            })).Verifiable();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _scheduleService.ApproveShiftSwap(id,1);
            });

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanRejectClaimedShift()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = userId,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Claimed
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShift(id)).Returns(Task.FromResult(new ScheduledShift()
            {
                Id = 1,
                StartsAt = DateTime.Now,
                EndsAt = DateTime.Now
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.UpdateScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Status == ShiftStatus.Released
            && y.ClaimedByUserId == null
            && y.AssignedUserId == 1
            && y.OriginalAssignedUserId == 1))).Verifiable();

            await _scheduleService.RejectShiftSwap(id, 1);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task RejectShiftClaimInInvalidStateThrows()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = null,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned
            })).Verifiable();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _scheduleService.RejectShiftSwap(id, 1);
            });

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCancelShiftRelease()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = userId,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Released
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.UpdateScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Status == ShiftStatus.Assigned
            && y.ClaimedByUserId == null
            && y.AssignedUserId == 1
            && y.OriginalAssignedUserId == 1))).Verifiable();

            await _scheduleService.CancelShiftRelease(id);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCancelClaimedShiftRelease()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = 1,
                ClaimedByUserId = userId,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Claimed
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.UpdateScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Status == ShiftStatus.Assigned
            && y.ClaimedByUserId == null
            && y.AssignedUserId == 1
            && y.OriginalAssignedUserId == 1))).Verifiable();

            await _scheduleService.CancelShiftRelease(id);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CancelCompletedShiftReleaseThrows()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignment(id)).Returns(Task.FromResult(new ScheduledShiftAssignment()
            {
                Id = 1,
                AssignedUserId = userId,
                ClaimedByUserId = null,
                OriginalAssignedUserId = 1,
                ScheduledShiftId = 1,
                Status = ShiftStatus.Assigned
            })).Verifiable();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _scheduleService.CancelShiftRelease(id);
            });

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCancelScheduledShiftWithAssignments()
        {
            int id = 1;
            int userId = 2;

            _shiftRepositoryMock.Setup(x => x.GetScheduledShift(id))
                .Returns(Task.FromResult(new ScheduledShift()
            {
                Id = 1,
                StartsAt = new DateTime(2001,1,1),
                EndsAt = new DateTime(2001,1,1),
                PatrolId = 1,
            })).Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignmentsForScheduledShift(id))
                .Returns(Task.FromResult(new List<ScheduledShiftAssignment>()
            {
                new ScheduledShiftAssignment()
                {
                    Id = 1,
                    AssignedUserId =1,
                    ClaimedByUserId = null,
                    OriginalAssignedUserId = 1,
                    ScheduledShiftId =1,
                    Status = ShiftStatus.Assigned
                }
            }.AsEnumerable())).Verifiable();

            _shiftRepositoryMock.Setup(x => x.DeleteScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.Id == 1)))
                .Verifiable();
            _shiftRepositoryMock.Setup(x => x.DeleteScheduledShift(It.Is<ScheduledShift>(y => y.Id == 1)))
                .Verifiable();

            await _scheduleService.CancelShift(id);

            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task ScheduleShiftCopiesGroupMemebersToShift()
        {
            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Test Patrol",
                TimeZone = "Eastern Standard Time"
            };
            _patrolRepository.Setup(x => x.GetPatrol(patrol.Id))
                .Returns(Task.FromResult(patrol))
                .Verifiable();

            var shift = new Shift()
            {
                Id = 1,
                Name="Morning",
                PatrolId = patrol.Id,
                StartHour = 1,
                StartMinute=2,
                EndHour = 13,
                EndMinute = 59
            };
            _shiftRepositoryMock.Setup(x => x.GetShift(shift.Id))
                .Returns(Task.FromResult(shift))
                .Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShifts(patrol.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<ScheduledShift>().AsEnumerable()))
                .Verifiable();

            var group = new Group()
            {
                Id = 1,
                Name = "Test Group",
                PatrolId = patrol.Id
            };
            var groupMembers = new List<User>()
            {
                new User()
                {
                    Id=1
                }
            };
            _groupRepositoryMock.Setup(x => x.GetGroup(group.Id))
                .Returns(Task.FromResult(group))
                .Verifiable();
            _groupRepositoryMock.Setup(x => x.GetUsersInGroup(group.Id))
                .Returns(Task.FromResult(groupMembers.AsEnumerable()))
                .Verifiable();

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShift(It.Is<ScheduledShift>(y => y.GroupId == group.Id)))
                .Verifiable();
            _shiftRepositoryMock.Setup(x => x.InsertScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.AssignedUserId == groupMembers.First().Id)))
                .Verifiable();

            await _scheduleService.ScheduleShift(new ScheduledShiftUpdateDto()
            {
                PatrolId = patrol.Id,
                ShiftId = shift.Id,
                GroupId = group.Id,
                Day = new DateTime(2001,1,1)
            });

            _patrolRepository.Verify();
            _groupRepositoryMock.Verify();
            _shiftRepositoryMock.Verify();

        }

        [Test]
        public async Task ScheduleShiftCopiesShiftTimesToShiftInUtc()
        {
            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Test Patrol",
                TimeZone = "Eastern Standard Time"
            };
            _patrolRepository.Setup(x => x.GetPatrol(patrol.Id))
                .Returns(Task.FromResult(patrol))
                .Verifiable();

            var shift = new Shift()
            {
                Id = 1,
                Name = "Morning",
                PatrolId = patrol.Id,
                StartHour = 1,
                StartMinute = 2,
                EndHour = 13,
                EndMinute = 59
            };
            _shiftRepositoryMock.Setup(x => x.GetShift(shift.Id))
                .Returns(Task.FromResult(shift))
                .Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShifts(patrol.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<ScheduledShift>().AsEnumerable()))
                .Verifiable();

            var group = new Group()
            {
                Id = 1,
                Name = "Test Group",
                PatrolId = patrol.Id
            };
            var groupMembers = new List<User>()
            {
                new User()
                {
                    Id=1
                }
            };
            _groupRepositoryMock.Setup(x => x.GetGroup(group.Id))
                .Returns(Task.FromResult(group))
                .Verifiable();
            _groupRepositoryMock.Setup(x => x.GetUsersInGroup(group.Id))
                .Returns(Task.FromResult(groupMembers.AsEnumerable()))
                .Verifiable();

            DateTime day = new DateTime(2001, 1, 1);

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShift(It.Is<ScheduledShift>(y => y.GroupId == group.Id
            && y.ShiftId == shift.Id
            && y.StartsAt == new DateTime(day.Year,day.Month,day.Day, 6,2,0,DateTimeKind.Local)
            && y.EndsAt == new DateTime(day.Year, day.Month, day.Day, 18, 59, 0, DateTimeKind.Local)))).Verifiable();

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.AssignedUserId == groupMembers.First().Id)))
                .Verifiable();

            await _scheduleService.ScheduleShift(new ScheduledShiftUpdateDto()
            {
                PatrolId = patrol.Id,
                ShiftId = shift.Id,
                GroupId = group.Id,
                Day = day
            });

            _patrolRepository.Verify();
            _groupRepositoryMock.Verify();
            _shiftRepositoryMock.Verify();

        }

        [Test]
        public async Task ScheduleShiftFindsExistingShiftByHoursIfNotSpecified()
        {
            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Test Patrol",
                TimeZone = "Eastern Standard Time"
            };
            _patrolRepository.Setup(x => x.GetPatrol(patrol.Id))
                .Returns(Task.FromResult(patrol))
                .Verifiable();

            var shift = new Shift()
            {
                Id = 1,
                Name = "Morning",
                PatrolId = patrol.Id,
                StartHour = 1,
                StartMinute = 2,
                EndHour = 13,
                EndMinute = 59
            };
            _shiftRepositoryMock.Setup(x => x.GetShifts(patrol.Id,shift.StartHour,shift.StartMinute,shift.EndHour,shift.EndMinute))
                .Returns(Task.FromResult(new List<Shift>() { shift }.AsEnumerable()))
                .Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShifts(patrol.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<ScheduledShift>().AsEnumerable()))
                .Verifiable();

            var group = new Group()
            {
                Id = 1,
                Name = "Test Group",
                PatrolId = patrol.Id
            };
            var groupMembers = new List<User>()
            {
                new User()
                {
                    Id=1
                }
            };
            _groupRepositoryMock.Setup(x => x.GetGroup(group.Id))
                .Returns(Task.FromResult(group))
                .Verifiable();
            _groupRepositoryMock.Setup(x => x.GetUsersInGroup(group.Id))
                .Returns(Task.FromResult(groupMembers.AsEnumerable()))
                .Verifiable();

            DateTime day = new DateTime(2001, 1, 1);
            DateTime start = new DateTime(2001, 1, 1, 6, 2, 0, DateTimeKind.Unspecified);
            DateTime end = new DateTime(2001, 1, 1, 18, 59, 0, DateTimeKind.Unspecified);

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShift(It.Is<ScheduledShift>(y => y.GroupId == group.Id
            && y.ShiftId == shift.Id
            && y.StartsAt == start
            && y.EndsAt == end))).Verifiable();

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.AssignedUserId == groupMembers.First().Id))).Verifiable();

            await _scheduleService.ScheduleShift(new ScheduledShiftUpdateDto()
            {
                PatrolId = patrol.Id,
                ShiftId = null,
                GroupId = group.Id,
                StartsAt = start,
                EndsAt = end
            });

            _patrolRepository.Verify();
            _groupRepositoryMock.Verify();
            _shiftRepositoryMock.Verify();

        }

        [Test]
        public async Task ScheduleShiftAddsUsersFromDto()
        {
            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Test Patrol",
                TimeZone = "Eastern Standard Time"
            };
            _patrolRepository.Setup(x => x.GetPatrol(patrol.Id))
                .Returns(Task.FromResult(patrol))
                .Verifiable();

            var shift = new Shift()
            {
                Id = 1,
                Name = "Morning",
                PatrolId = patrol.Id,
                StartHour = 1,
                StartMinute = 2,
                EndHour = 13,
                EndMinute = 59
            };
            _shiftRepositoryMock.Setup(x => x.GetShift(shift.Id))
                .Returns(Task.FromResult(shift))
                .Verifiable();

            _shiftRepositoryMock.Setup(x => x.GetScheduledShifts(patrol.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<ScheduledShift>().AsEnumerable()))
                .Verifiable();

            DateTime day = new DateTime(2001, 1, 1);

            var assignUserIds = new List<int>() { 1 };

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShift(It.Is<ScheduledShift>(y => y.GroupId == null
            && y.ShiftId == shift.Id
            && y.StartsAt == new DateTime(day.Year, day.Month, day.Day, 6, 2, 0, DateTimeKind.Local)
            && y.EndsAt == new DateTime(day.Year, day.Month, day.Day, 18, 59, 0, DateTimeKind.Local)))).Verifiable();

            _shiftRepositoryMock.Setup(x => x.InsertScheduledShiftAssignment(It.Is<ScheduledShiftAssignment>(y => y.AssignedUserId == assignUserIds.First())))
                .Verifiable();

            await _scheduleService.ScheduleShift(new ScheduledShiftUpdateDto()
            {
                PatrolId = patrol.Id,
                ShiftId = shift.Id,
                GroupId = null,
                Day = day,
                AssignUserIds = assignUserIds
            });

            _patrolRepository.Verify();
            _groupRepositoryMock.Verify();
            _shiftRepositoryMock.Verify();

        }
    }
}
