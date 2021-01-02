using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Mappings;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Tests.Services
{
    public class WorkItemServiceTests
    {
        private WorkItemService _workItemService;
        private Mock<IGroupRepository> _groupRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IPatrolRepository> _patrolRepositoryMock;
        private Mock<IShiftRepository> _shiftRepositoryMock;
        private Mock<IWorkItemRepository> _workItemRepository;
        private Mock<ILogger<WorkItemService>> _loggerMock;
        private Mock<ISystemClock> _systemClockMock;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<WorkItemService>>();
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _patrolRepositoryMock = new Mock<IPatrolRepository>();
            _shiftRepositoryMock = new Mock<IShiftRepository>();
            _workItemRepository = new Mock<IWorkItemRepository>();
            _systemClockMock = new Mock<ISystemClock>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _workItemService = new WorkItemService(_loggerMock.Object,_patrolRepositoryMock.Object,_groupRepositoryMock.Object,_shiftRepositoryMock.Object,_systemClockMock.Object,
                _emailServiceMock.Object,_userRepositoryMock.Object,_workItemRepository.Object,_mapper);
        }

        //create one off work item
        //cancel work item
        //complete any
        //complete all assigned
        //complete any assigned

        //update work item

        //save recurring work item (shift + auto)
        //save recurring work item (shift + previous)
        //save recurring work item (shift + manual)
        //save recurring work item (recur)

        [Test]
        public async Task CanCreateOneOffWorkItem()
        {
            var workItemDto = new WorkItemDto();
            workItemDto.AdminGroupId = null;
            workItemDto.Assignments = new List<WorkItemAssignmentDto>();
            workItemDto.Assignments.Add(new WorkItemAssignmentDto()
            {
                UserId = 2
            });
            workItemDto.CompletionMode = Api.Models.CompletionMode.AnyAssigned;
            workItemDto.DescriptionMarkup = "<h1>header</h1>";
            workItemDto.Location = "location";
            workItemDto.Name = "name";
            workItemDto.PatrolId = 1;
            workItemDto.RecurringWorkItemId = null;
            workItemDto.ScheduledAt = new DateTime(2001, 1, 1);
            int userId = 50;

            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.Name == workItemDto.Name)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.UserId == 2)))
                .Verifiable();

            await _workItemService.SaveWorkItem(workItemDto,userId);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCancelWorkItem()
        {
            var workItemDto = new WorkItem();
            workItemDto.Id = 2;
            workItemDto.AdminGroupId = null;
            workItemDto.CompletionMode = Api.Models.CompletionMode.AnyAssigned;
            workItemDto.DescriptionMarkup = "<h1>header</h1>";
            workItemDto.Location = "location";
            workItemDto.Name = "name";
            workItemDto.PatrolId = 1;
            workItemDto.RecurringWorkItemId = null;
            workItemDto.ScheduledAt = new DateTime(2001, 1, 1);
            int userId = 50;

            _workItemRepository.Setup(x => x.GetWorkItem(workItemDto.Id))
                .Returns(Task.FromResult(workItemDto))
                .Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            _workItemRepository.Setup(x => x.UpdateWorkItem(It.Is<WorkItem>(y => y.CanceledAt == now.UtcDateTime)))
                .Verifiable();

            await _workItemService.CancelWorkItem(workItemDto.Id, userId);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCompleteWorkItemAny()
        {
            var workItemDto = new WorkItem();
            workItemDto.Id = 2;
            workItemDto.AdminGroupId = null;
            workItemDto.CompletionMode = Api.Models.CompletionMode.Any;
            workItemDto.DescriptionMarkup = "<h1>header</h1>";
            workItemDto.Location = "location";
            workItemDto.Name = "name";
            workItemDto.PatrolId = 1;
            workItemDto.RecurringWorkItemId = null;
            workItemDto.ScheduledAt = new DateTime(2001, 1, 1);
            int userId = 50;

            _workItemRepository.Setup(x => x.GetWorkItem(workItemDto.Id))
                .Returns(Task.FromResult(workItemDto))
                .Verifiable();

            _workItemRepository.Setup(x => x.GetWorkItemAssignments(workItemDto.Id))
                .Returns(Task.FromResult((new List<WorkItemAssignment>()).AsEnumerable())).Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            _workItemRepository.Setup(x => x.UpdateWorkItem(It.Is<WorkItem>(y => y.CompletedAt == now.UtcDateTime)))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.CompletedAt == now.UtcDateTime)))
                .Verifiable();

            var notes = "notes";

            await _workItemService.CompleteWorkItem(workItemDto.Id, userId,notes);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCompleteWorkItemAllAssigned()
        {
            var workItemDto = new WorkItem();
            workItemDto.Id = 2;
            workItemDto.AdminGroupId = null;
            workItemDto.CompletionMode = Api.Models.CompletionMode.AllAssigned;
            workItemDto.DescriptionMarkup = "<h1>header</h1>";
            workItemDto.Location = "location";
            workItemDto.Name = "name";
            workItemDto.PatrolId = 1;
            workItemDto.RecurringWorkItemId = null;
            workItemDto.ScheduledAt = new DateTime(2001, 1, 1);
            int userIdA = 50;
            int userIdB = 51;

            _workItemRepository.Setup(x => x.GetWorkItem(workItemDto.Id))
                .Returns(Task.FromResult(workItemDto))
                .Verifiable();

            _workItemRepository.Setup(x => x.GetWorkItemAssignments(workItemDto.Id))
                .Returns(Task.FromResult((new List<WorkItemAssignment>()
                {
                    new WorkItemAssignment()
                    {
                        WorkItemId = workItemDto.Id,
                        UserId = userIdA
                    },
                    new WorkItemAssignment()
                    {
                        WorkItemId = workItemDto.Id,
                        UserId = userIdB
                    }
                }).AsEnumerable())).Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            

            _workItemRepository.Setup(x => x.UpdateWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.CompletedAt == now.UtcDateTime && y.UserId==userIdA)))
                .Verifiable();

            var notes = "notes";

            await _workItemService.CompleteWorkItem(workItemDto.Id, userIdA, notes);

            _workItemRepository.Verify();

            _workItemRepository.Setup(x => x.UpdateWorkItem(It.Is<WorkItem>(y => y.CompletedAt == now.UtcDateTime)))
                .Verifiable();

            _workItemRepository.Setup(x => x.UpdateWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.CompletedAt == now.UtcDateTime && y.UserId == userIdB)))
                .Verifiable();

            await _workItemService.CompleteWorkItem(workItemDto.Id, userIdB, notes);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCompleteWorkItemAnyAssigned()
        {
            var workItemDto = new WorkItem();
            workItemDto.Id = 2;
            workItemDto.AdminGroupId = null;
            workItemDto.CompletionMode = Api.Models.CompletionMode.AnyAssigned;
            workItemDto.DescriptionMarkup = "<h1>header</h1>";
            workItemDto.Location = "location";
            workItemDto.Name = "name";
            workItemDto.PatrolId = 1;
            workItemDto.RecurringWorkItemId = null;
            workItemDto.ScheduledAt = new DateTime(2001, 1, 1);
            int userId = 50;

            _workItemRepository.Setup(x => x.GetWorkItem(workItemDto.Id))
                .Returns(Task.FromResult(workItemDto))
                .Verifiable();

            _workItemRepository.Setup(x => x.GetWorkItemAssignments(workItemDto.Id))
                .Returns(Task.FromResult((new List<WorkItemAssignment>()
                {
                    new WorkItemAssignment()
                    {
                        WorkItemId = workItemDto.Id,
                        UserId = userId
                    }
                }).AsEnumerable())).Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            _workItemRepository.Setup(x => x.UpdateWorkItem(It.Is<WorkItem>(y => y.CompletedAt == now.UtcDateTime)))
                .Verifiable();

            _workItemRepository.Setup(x => x.UpdateWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.CompletedAt == now.UtcDateTime)))
                .Verifiable();

            var notes = "notes";

            await _workItemService.CompleteWorkItem(workItemDto.Id, userId, notes);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanUpdateWorkItem()
        {
            var workItemDto = new WorkItemDto();
            workItemDto.Id = 1;
            workItemDto.AdminGroupId = null;
            workItemDto.Assignments = new List<WorkItemAssignmentDto>();
            workItemDto.Assignments.Add(new WorkItemAssignmentDto()
            {
                UserId = 2
            });
            workItemDto.CompletionMode = Api.Models.CompletionMode.AnyAssigned;
            workItemDto.DescriptionMarkup = "<h1>header</h1>";
            workItemDto.Location = "location";
            workItemDto.Name = "name";
            workItemDto.PatrolId = 1;
            workItemDto.RecurringWorkItemId = null;
            workItemDto.ScheduledAt = new DateTime(2001, 1, 1);
            int userId = 50;

            var wi = _mapper.Map<WorkItemDto, WorkItem>(workItemDto);

            _workItemRepository.Setup(x => x.GetWorkItem(workItemDto.Id))
                .Returns(Task.FromResult(wi))
                .Verifiable();
            _workItemRepository.Setup(x => x.UpdateWorkItem(It.Is<WorkItem>(y => y.Id==workItemDto.Id)))
                .Verifiable();
            _workItemRepository.Setup(x => x.GetWorkItemAssignments(workItemDto.Id))
                .Returns(Task.FromResult(new List<WorkItemAssignment>()
                {
                    new WorkItemAssignment()
                    {
                        WorkItemId = workItemDto.Id,
                        UserId = 3
                    }
                }.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.DeleteWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.UserId == 3)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.UserId == 2)))
                .Verifiable();

            await _workItemService.SaveWorkItem(workItemDto, userId);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCreateShiftRecurringWorkItem()
        {
            
            var dto = new RecurringWorkItemDto();
            int userId = 1;

            var shift = new Shift()
            {
                Name = "Shift",
                Id = 60,
                StartHour = 9,
                EndHour = 17
            };

            dto.CompletionMode = CompletionMode.AnyAssigned;
            dto.DescriptionMarkup = "Description";
            dto.Location = "Location";
            dto.Name = "Name";
            dto.PatrolId = 1;
            dto.Shifts = new List<ShiftRecurringWorkItemDto>();
            var shiftRecurringWorkItem = new ShiftRecurringWorkItemDto()
            {
                ScheduledAtHour = 10,
                ScheduledAtMinute = 0,
                ShiftAssignmentMode = ShiftAssignmentMode.Auto,
                ShiftId = shift.Id
            };
            dto.Shifts.Add(shiftRecurringWorkItem);

            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Patrol",
                TimeZone = TimeZoneInfo.Local.Id
            };
            _patrolRepositoryMock.Setup(x => x.GetPatrol(patrol.Id))
                .Returns(Task.FromResult(patrol))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertShiftRecurringWorkItem(It.Is<ShiftRecurringWorkItem>(y => y.ShiftId == shiftRecurringWorkItem.ShiftId)))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertRecurringWorkItem(It.Is<RecurringWorkItem>(y => y.Name == dto.Name)))
                .Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            await _workItemService.SaveRecurringWorkItem(dto, userId,false,false);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCreateWorkItemsFromShiftRecurringWorkitem()
        {
            var dto = new RecurringWorkItemDto();
            int userId = 1;

            var shift = new Shift()
            {
                Name = "Shift",
                Id = 60,
                StartHour = 9,
                EndHour = 17
            };

            dto.CompletionMode = CompletionMode.AnyAssigned;
            dto.DescriptionMarkup = "Description";
            dto.Location = "Location";
            dto.Name = "Name";
            dto.PatrolId = 1;
            dto.Shifts = new List<ShiftRecurringWorkItemDto>();
            var shiftRecurringWorkItem = new ShiftRecurringWorkItemDto()
            {
                ScheduledAtHour = 10,
                ScheduledAtMinute = 0,
                ShiftAssignmentMode = ShiftAssignmentMode.Auto,
                ShiftId = shift.Id
            };
            dto.Shifts.Add(shiftRecurringWorkItem);

            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Patrol",
                TimeZone = TimeZoneInfo.Local.Id
            };
            
            _workItemRepository.Setup(x => x.GetShiftRecurringWorkItems(It.IsAny<int>()))
                .Returns(Task.FromResult(new List<ShiftRecurringWorkItem>() {
                    new ShiftRecurringWorkItem()
                    {
                        RecurringWorkItemId = 0,
                        ScheduledAtHour = 9,
                        ScheduledAtMinute = 0,
                        ShiftAssignmentMode = ShiftAssignmentMode.Auto,
                        ShiftId = shift.Id
                    }
                }.AsEnumerable()))
                .Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            _workItemRepository.Setup(x => x.GetWorkItems(It.IsAny<int>(), now.UtcDateTime))
                .Returns(Task.FromResult(new List<WorkItem>() {
                }.AsEnumerable()))
                .Verifiable();

            var assignments = new List<ScheduledShiftAssignmentDto>();
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 1,
                AssignedUser = new UserIdentifier() { Id = 1, FirstName = "1" },
                StartsAt = new DateTime(2001, 1, 1, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 17, 0, 0),
                ScheduledShiftId = 1,
                Shift = shift
            });
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 2,
                AssignedUser = new UserIdentifier() { Id = 2, FirstName = "2" },
                StartsAt = new DateTime(2001, 1, 1, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 17, 0, 0),
                ScheduledShiftId = 1,
                Shift = shift
            });
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 3,
                AssignedUser = new UserIdentifier() { Id = 1, FirstName = "1" },
                StartsAt = new DateTime(2001, 1, 2, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 2, 17, 0, 0),
                ScheduledShiftId = 2,
                Shift = shift
            });
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 4,
                AssignedUser = new UserIdentifier() { Id = 2, FirstName = "2" },
                StartsAt = new DateTime(2001, 1, 2, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 2, 17, 0, 0),
                ScheduledShiftId = 2,
                Shift = shift
            });

            _shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignments(dto.PatrolId, null, now.UtcDateTime, null, null, null, null, dto.Shifts.First().ShiftId))
                .Returns(Task.FromResult(assignments.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledShiftId == 1)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledShiftId == 2)))
                .Verifiable();

            await _workItemService.PopulateShiftWorkItemOccurences(dto,now.UtcDateTime, userId, patrol, false);

            _workItemRepository.Verify();
            _shiftRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCreateWorkItemAssignmentsForRecurringWorkitem()
        {
            var dto = new RecurringWorkItemDto();
            int userId = 1;

            var shift = new Shift()
            {
                Name = "Shift",
                Id = 60,
                StartHour = 9,
                EndHour = 17
            };

            dto.CompletionMode = CompletionMode.AnyAssigned;
            dto.DescriptionMarkup = "Description";
            dto.Location = "Location";
            dto.Name = "Name";
            dto.PatrolId = 1;
            dto.Shifts = new List<ShiftRecurringWorkItemDto>();
            var shiftRecurringWorkItem = new ShiftRecurringWorkItemDto()
            {
                ScheduledAtHour = 10,
                ScheduledAtMinute = 0,
                ShiftAssignmentMode = ShiftAssignmentMode.Auto,
                ShiftId = shift.Id
            };
            dto.Shifts.Add(shiftRecurringWorkItem);

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            var assignments = new List<ScheduledShiftAssignmentDto>();
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 1,
                AssignedUser = new UserIdentifier() { Id = 1, FirstName = "1" },
                StartsAt = new DateTime(2001, 1, 1, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 17, 0, 0),
                ScheduledShiftId = 1,
                Shift = shift
            });
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 2,
                AssignedUser = new UserIdentifier() { Id = 2, FirstName = "2" },
                StartsAt = new DateTime(2001, 1, 1, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 17, 0, 0),
                ScheduledShiftId = 1,
                Shift = shift
            });
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 3,
                AssignedUser = new UserIdentifier() { Id = 1, FirstName = "1" },
                StartsAt = new DateTime(2001, 1, 2, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 2, 17, 0, 0),
                ScheduledShiftId = 2,
                Shift = shift
            });
            assignments.Add(new ScheduledShiftAssignmentDto()
            {
                Id = 4,
                AssignedUser = new UserIdentifier() { Id = 2, FirstName = "2" },
                StartsAt = new DateTime(2001, 1, 2, 9, 0, 0),
                EndsAt = new DateTime(2001, 1, 2, 17, 0, 0),
                ScheduledShiftId = 2,
                Shift = shift
            });

            //_shiftRepositoryMock.Setup(x => x.GetScheduledShiftAssignments(dto.PatrolId, null, now.UtcDateTime, null, null, null, null, dto.Shifts.First().ShiftId))
            //    .Returns(Task.FromResult(assignments.AsEnumerable()))
            //    .Verifiable();

            _workItemRepository.Setup(x => x.GetWorkItemsForShifts(It.Is<List<int>>(x => assignments.Select(x => x.ScheduledShiftId).Distinct().ToArray().SequenceEqual(x.ToArray()))))
                .Returns(Task.FromResult(new List<WorkItem>() {
                    new WorkItem()
                    {
                        Id=1,
                        ScheduledShiftId = 1,
                        RecurringWorkItemId = 1,
                        CompletionMode = dto.CompletionMode,
                        DescriptionMarkup = dto.DescriptionMarkup,
                        Name = dto.Name,
                        Location = dto.Location,
                        ScheduledAt = new DateTime(2001,1,1,9,0,0)
                    },
                    new WorkItem()
                    {
                        Id=2,
                        ScheduledShiftId = 2,
                        RecurringWorkItemId = 1,
                        CompletionMode = dto.CompletionMode,
                        DescriptionMarkup = dto.DescriptionMarkup,
                        Name = dto.Name,
                        Location = dto.Location,
                        ScheduledAt = new DateTime(2001,1,2,9,0,0)
                    }
                }.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.GetWorkItemAssignmentsForShifts(It.Is<List<int>>(x => assignments.Select(x => x.ScheduledShiftId).Distinct().ToArray().SequenceEqual(x.ToArray()))))
                .Returns(Task.FromResult(new List<WorkItemAssignment>().AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.GetRecurringWorkItemsForShifts(It.Is<List<int>>(x => assignments.Select(x => x.ScheduledShiftId).Distinct().ToArray().SequenceEqual(x.ToArray()))))
                .Returns(Task.FromResult(new List<RecurringWorkItem>() { new RecurringWorkItem() {
                    CompletionMode = dto.CompletionMode,
                    DescriptionMarkup = dto.DescriptionMarkup,
                    Id = 1,
                    Location = dto.Location,
                    Name = dto.Name,
                    PatrolId = dto.PatrolId,
                } }.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.GetShiftRecurringWorkItemsForShifts(It.Is<List<int>>(x => assignments.Select(x => x.ScheduledShiftId).Distinct().ToArray().SequenceEqual(x.ToArray()))))
                .Returns(Task.FromResult(new List<ShiftRecurringWorkItem>() { new ShiftRecurringWorkItem() {
                    Id = 1,
                    RecurringWorkItemId = 1,
                    ScheduledAtHour = shiftRecurringWorkItem.ScheduledAtHour,
                    ScheduledAtMinute = shiftRecurringWorkItem.ScheduledAtMinute,
                    ShiftAssignmentMode = shiftRecurringWorkItem.ShiftAssignmentMode,
                    ShiftId = shift.Id
                } }.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.UserId == 1)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.UserId == 2)))
                .Verifiable();


            await _workItemService.RecalculateShiftWorkItemAssignments(assignments);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCreateRecurringWorkItem()
        {
            var dto = new RecurringWorkItemDto();
            int userId = 1;

            dto.CompletionMode = CompletionMode.AnyAssigned;
            dto.DescriptionMarkup = "Description";
            dto.Location = "Location";
            dto.Name = "Name";
            dto.PatrolId = 1;
            dto.RecurStart = new DateTime(2001, 1, 1, 9, 0, 0);
            dto.RecurEnd = new DateTime(2001, 1, 10, 9, 0, 0);
            dto.RecurIntervalSeconds = (int)new TimeSpan(24, 0, 0).TotalSeconds;
            
            var patrol = new Api.Models.Patrol()
            {
                Id = 1,
                Name = "Patrol",
                TimeZone = TimeZoneInfo.Local.Id
            };
            _patrolRepositoryMock.Setup(x => x.GetPatrol(patrol.Id))
                .Returns(Task.FromResult(patrol))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertRecurringWorkItem(It.Is<RecurringWorkItem>(y => y.Name == dto.Name)))
                .Verifiable();

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            await _workItemService.SaveRecurringWorkItem(dto, userId, false, false);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCreateWorkItemsFromRecurringWorkItem()
        {
            var dto = new RecurringWorkItem();
            int userId = 1;

            dto.CompletionMode = CompletionMode.AnyAssigned;
            dto.DescriptionMarkup = "Description";
            dto.Location = "Location";
            dto.Name = "Name";
            dto.PatrolId = 1;
            dto.RecurStart = new DateTime(2001, 1, 1, 9, 0, 0);
            dto.RecurEnd = new DateTime(2001, 1, 4, 9, 0, 0);
            dto.RecurIntervalSeconds = (int)new TimeSpan(24, 0, 0).TotalSeconds;

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            _workItemRepository.Setup(x => x.GetWorkItems(It.IsAny<int>(), now.UtcDateTime))
                .Returns(Task.FromResult(new List<WorkItem>()
                {
                }.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledAt.Day == 1)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledAt.Day == 2)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledAt.Day == 3)))
                .Verifiable();
            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledAt.Day == 4)))
                .Verifiable();

            await _workItemService.PopulateTimedWorkItemOccurences(dto, userId,new List<int>() { userId },now.UtcDateTime, false);

            _workItemRepository.Verify();
        }

        [Test]
        public async Task CanCreateWorkItemAssignmentsFromRecurringWorkItem()
        {
            var dto = new RecurringWorkItem();
            int userId = 1;

            dto.CompletionMode = CompletionMode.AnyAssigned;
            dto.DescriptionMarkup = "Description";
            dto.Location = "Location";
            dto.Name = "Name";
            dto.PatrolId = 1;
            dto.RecurStart = new DateTime(2001, 1, 1, 9, 0, 0);
            dto.RecurEnd = new DateTime(2001, 1, 1, 9, 0, 0);
            dto.RecurIntervalSeconds = (int)new TimeSpan(24, 0, 0).TotalSeconds;

            var now = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan());
            _systemClockMock.Setup(x => x.UtcNow).Returns(now);

            _workItemRepository.Setup(x => x.GetWorkItems(It.IsAny<int>(), now.UtcDateTime))
                .Returns(Task.FromResult(new List<WorkItem>()
                {
                }.AsEnumerable()))
                .Verifiable();

            _workItemRepository.Setup(x => x.InsertWorkItem(It.Is<WorkItem>(y => y.ScheduledAt.Day == 1)))
                .Verifiable();
            
            
            _workItemRepository.Setup(x => x.InsertWorkItemAssignment(It.Is<WorkItemAssignment>(y => y.UserId == userId)))
                .Verifiable();

            await _workItemService.PopulateTimedWorkItemOccurences(dto, userId, new List<int>() { userId }, now.UtcDateTime,true);

            _workItemRepository.Verify();
        }
    }
}
