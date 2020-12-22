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
    public class WorkItemRepositoryTests : DatabaseConnectedTestFixture
    {
        private WorkItemRepository _workItemRepository;

        [SetUp]
        public void SetUp()
        {
            _workItemRepository = new WorkItemRepository(_connection);
        }

        [Test]
        public async Task CanInsertRecurringWorkItem()
        {
            var recurringWorkItem = new RecurringWorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                Name="Name",
                Location="Location",
                ShiftAssignmentMode = ShiftAssignmentMode.ManualEachShift,
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertRecurringWorkItem(recurringWorkItem);

            Assert.NotZero(recurringWorkItem.Id);
        }

        [Test]
        public async Task CanRetrieveInsertedRecurringWorkItem()
        {
            var recurringWorkItem = new RecurringWorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                ShiftAssignmentMode = ShiftAssignmentMode.ManualEachShift,
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertRecurringWorkItem(recurringWorkItem);


            var inserted = await _workItemRepository.GetRecurringWorkItem(recurringWorkItem.Id);

            Assert.AreEqual(recurringWorkItem.PatrolId, inserted.PatrolId);
            Assert.AreEqual(recurringWorkItem.CreatedByUserId, inserted.CreatedByUserId);
            Assert.AreEqual(recurringWorkItem.Name, inserted.Name);
            Assert.AreEqual(recurringWorkItem.Location, inserted.Location);
            Assert.AreEqual(recurringWorkItem.ShiftAssignmentMode, inserted.ShiftAssignmentMode);
            Assert.AreEqual(recurringWorkItem.CompletionMode, inserted.CompletionMode);
            Assert.AreEqual(recurringWorkItem.DescriptionMarkup, inserted.DescriptionMarkup);
            Assert.AreEqual(recurringWorkItem.AdminGroupId, inserted.AdminGroupId);
        }

        [Test]
        public async Task CanUpdateRecurringWorkItem()
        {
            var recurringWorkItem = new RecurringWorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                ShiftAssignmentMode = ShiftAssignmentMode.ManualEachShift,
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertRecurringWorkItem(recurringWorkItem);
            recurringWorkItem.PatrolId = 2;
            recurringWorkItem.CreatedAt = DateTime.Now;
            recurringWorkItem.CreatedByUserId = 2;
            recurringWorkItem.Name = "Name2";
            recurringWorkItem.Location = "Location2";
            recurringWorkItem.ShiftAssignmentMode = ShiftAssignmentMode.CopyPrevious;
            recurringWorkItem.CompletionMode = CompletionMode.AllAssigned;
            recurringWorkItem.DescriptionMarkup = "Markup2";
            recurringWorkItem.AdminGroupId = 2;

            await _workItemRepository.UpdateRecurringWorkItem(recurringWorkItem);

            var inserted = await _workItemRepository.GetRecurringWorkItem(recurringWorkItem.Id);

            Assert.AreEqual(recurringWorkItem.PatrolId, inserted.PatrolId);
            Assert.AreEqual(recurringWorkItem.CreatedByUserId, inserted.CreatedByUserId);
            Assert.AreEqual(recurringWorkItem.Name, inserted.Name);
            Assert.AreEqual(recurringWorkItem.Location, inserted.Location);
            Assert.AreEqual(recurringWorkItem.ShiftAssignmentMode, inserted.ShiftAssignmentMode);
            Assert.AreEqual(recurringWorkItem.CompletionMode, inserted.CompletionMode);
            Assert.AreEqual(recurringWorkItem.DescriptionMarkup, inserted.DescriptionMarkup);
            Assert.AreEqual(recurringWorkItem.AdminGroupId, inserted.AdminGroupId);

        }

        [Test]
        public async Task CanInsertWorkItem()
        {
            var workItem = new WorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                ScheduledAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertWorkItem(workItem);

            Assert.NotZero(workItem.Id);
        }

        [Test]
        public async Task CanRetrieveInsertedWorkItem()
        {
            var workItem = new WorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                ScheduledAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertWorkItem(workItem);


            var inserted = await _workItemRepository.GetWorkItem(workItem.Id);

            Assert.AreEqual(workItem.PatrolId, inserted.PatrolId);
            Assert.AreEqual(workItem.CreatedByUserId, inserted.CreatedByUserId);
            Assert.AreEqual(workItem.Name, inserted.Name);
            Assert.AreEqual(workItem.Location, inserted.Location);
            Assert.AreEqual(workItem.CompletionMode, inserted.CompletionMode);
            Assert.AreEqual(workItem.DescriptionMarkup, inserted.DescriptionMarkup);
            Assert.AreEqual(workItem.AdminGroupId, inserted.AdminGroupId);
        }

        [Test]
        public async Task CanUpdateWorkItem()
        {
            var workItem = new WorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                ScheduledAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertWorkItem(workItem);
            workItem.PatrolId = 2;
            workItem.CreatedAt = DateTime.Now;
            workItem.CreatedByUserId = 2;
            workItem.Name = "Name2";
            workItem.Location = "Location2";
            workItem.CompletionMode = CompletionMode.AllAssigned;
            workItem.DescriptionMarkup = "Markup2";
            workItem.AdminGroupId = 2;

            await _workItemRepository.UpdateWorkItem(workItem);

            var inserted = await _workItemRepository.GetWorkItem(workItem.Id);

            Assert.AreEqual(workItem.PatrolId, inserted.PatrolId);
            Assert.AreEqual(workItem.CreatedByUserId, inserted.CreatedByUserId);
            Assert.AreEqual(workItem.Name, inserted.Name);
            Assert.AreEqual(workItem.Location, inserted.Location);
            Assert.AreEqual(workItem.CompletionMode, inserted.CompletionMode);
            Assert.AreEqual(workItem.DescriptionMarkup, inserted.DescriptionMarkup);
            Assert.AreEqual(workItem.AdminGroupId, inserted.AdminGroupId);

        }

        [Test]
        public async Task CanInsertShiftRecurringWorkItem()
        {
            var recurringWorkItem = new RecurringWorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                ShiftAssignmentMode = ShiftAssignmentMode.ManualEachShift,
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };
            
            await _workItemRepository.InsertRecurringWorkItem(recurringWorkItem);

            var shiftRecurringWorkItem = new ShiftRecurringWorkItem()
            {
                RecurringWorkItemId = recurringWorkItem.Id,
                ShiftId = 1,
                ScheduledAtHour = 9,
                ScheduledAtMinute = 0
            };

            await _workItemRepository.InsertShiftRecurringWorkItem(shiftRecurringWorkItem);

            Assert.NotZero(shiftRecurringWorkItem.Id);
        }

        [Test]
        public async Task CanRetrieveInsertedShiftRecurringWorkItem()
        {
            var recurringWorkItem = new RecurringWorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                ShiftAssignmentMode = ShiftAssignmentMode.ManualEachShift,
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertRecurringWorkItem(recurringWorkItem);

            var shiftRecurringWorkItem = new ShiftRecurringWorkItem()
            {
                RecurringWorkItemId = recurringWorkItem.Id,
                ShiftId = 1,
                ScheduledAtHour = 9,
                ScheduledAtMinute = 0
            };

            await _workItemRepository.InsertShiftRecurringWorkItem(shiftRecurringWorkItem);

            var inserted = await _workItemRepository.GetShiftRecurringWorkItem(shiftRecurringWorkItem.Id);

            Assert.AreEqual(shiftRecurringWorkItem.RecurringWorkItemId, inserted.RecurringWorkItemId);
            Assert.AreEqual(shiftRecurringWorkItem.ShiftId, inserted.ShiftId);
            Assert.AreEqual(shiftRecurringWorkItem.ScheduledAtHour, inserted.ScheduledAtHour);
            Assert.AreEqual(shiftRecurringWorkItem.ScheduledAtMinute, inserted.ScheduledAtMinute);
            
        }

        [Test]
        public async Task CanUpdateShiftRecurringWorkItem()
        {
            var recurringWorkItem = new RecurringWorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                ShiftAssignmentMode = ShiftAssignmentMode.ManualEachShift,
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };

            await _workItemRepository.InsertRecurringWorkItem(recurringWorkItem);

            var shiftRecurringWorkItem = new ShiftRecurringWorkItem()
            {
                RecurringWorkItemId = recurringWorkItem.Id,
                ShiftId = 1,
                ScheduledAtHour = 9,
                ScheduledAtMinute = 0
            };

            await _workItemRepository.InsertShiftRecurringWorkItem(shiftRecurringWorkItem);

            shiftRecurringWorkItem.ShiftId = 2;
            shiftRecurringWorkItem.ScheduledAtHour = 10;
            shiftRecurringWorkItem.ScheduledAtMinute = 1;

            await _workItemRepository.UpdateShiftRecurringWorkItem(shiftRecurringWorkItem);

            var inserted = await _workItemRepository.GetShiftRecurringWorkItem(shiftRecurringWorkItem.Id);

            Assert.AreEqual(shiftRecurringWorkItem.RecurringWorkItemId, inserted.RecurringWorkItemId);
            Assert.AreEqual(shiftRecurringWorkItem.ShiftId, inserted.ShiftId);
            Assert.AreEqual(shiftRecurringWorkItem.ScheduledAtHour, inserted.ScheduledAtHour);
            Assert.AreEqual(shiftRecurringWorkItem.ScheduledAtMinute, inserted.ScheduledAtMinute);

        }

        [Test]
        public async Task CanInsertWorkItemAssignment()
        {
            var workItem = new WorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                ScheduledAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };
            await _workItemRepository.InsertWorkItem(workItem);

            var assignment = new WorkItemAssignment()
            {
                UserId = 1,
                WorkItemId = workItem.Id,
            };

            await _workItemRepository.InsertWorkItemAssignment(assignment);


            Assert.NotZero(assignment.Id);
        }

        [Test]
        public async Task CanRetrieveInsertedWorkItemAssignment()
        {
            var workItem = new WorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                ScheduledAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };
            await _workItemRepository.InsertWorkItem(workItem);

            var assignment = new WorkItemAssignment()
            {
                UserId = 1,
                WorkItemId = workItem.Id,
            };

            await _workItemRepository.InsertWorkItemAssignment(assignment);


            var inserted = await _workItemRepository.GetWorkItemAssignment(assignment.Id);

            Assert.AreEqual(assignment.UserId, inserted.UserId);
            Assert.AreEqual(assignment.WorkItemId, inserted.WorkItemId);
        }

        [Test]
        public async Task CanUpdateWorkItemAssignment()
        {
            var workItem = new WorkItem()
            {
                PatrolId = 1,
                CreatedAt = DateTime.Now,
                ScheduledAt = DateTime.Now,
                CreatedByUserId = 1,
                Name = "Name",
                Location = "Location",
                CompletionMode = CompletionMode.Any,
                DescriptionMarkup = "Markup",
                AdminGroupId = 1
            };
            await _workItemRepository.InsertWorkItem(workItem);

            var assignment = new WorkItemAssignment()
            {
                UserId = 1,
                WorkItemId = workItem.Id,
            };

            await _workItemRepository.InsertWorkItemAssignment(assignment);

            assignment.UserId = 2;
            await _workItemRepository.UpdateWorkItemAssignment(assignment);


            var inserted = await _workItemRepository.GetWorkItemAssignment(assignment.Id);

            Assert.AreEqual(assignment.UserId, inserted.UserId);
            Assert.AreEqual(assignment.WorkItemId, inserted.WorkItemId);

        }
    }
}
