using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IWorkItemService
    {
        Task SaveRecurringWorkItem(RecurringWorkItemDto recurringWorkItem, int userId, bool populateWorkItems = true, bool populateWorkItemAssignments = true, DateTime? overrideNow = null);
        Task<RecurringWorkItemDto> GetRecurringWorkItem(int id);
        Task EndRecurringWorkItem(int id);
        Task SaveWorkItem(WorkItemDto workItem, int userId);
        Task ReassignWorkItem(WorkItemDto workItem);
        Task<WorkItemDto> GetWorkItem(int id);
        Task CompleteWorkItem(int workItemId, int userId, string workNotes, int? onBehalfOfUserId = null, bool force = false);
        Task CancelWorkItem(int workItemId, int userId);
        Task<bool> CanCompleteWorkItem(int workItemId, int userId);
        Task<bool> CanCancelWorkItem(int workItemId, int userId);

        //methods that probably ought to just be internal, but are public for testing
        Task PopulateShiftWorkItemOccurences(RecurringWorkItemDto recurringWorkItem, DateTime now, int userId, Models.Patrol patrol, bool populateWorkitemAssignments = true);
        Task RecalculateShiftWorkItemAssignments(List<ScheduledShiftAssignmentDto> scheduledShiftAssignments);
        Task PopulateTimedWorkItemOccurences(RecurringWorkItem workItem, int userId, List<int> assigneeUserIds, DateTime now, bool populateWorkItemAssignments = true);

    }

    public interface IShiftWorkItemService
    {
        Task AddWorkItemsToNewShiftOccurence(ScheduledShift shift);
        Task RemoveWorkItemsFromShiftOccurence(ScheduledShift shift);
        Task SwapScheduledShiftWorkItems(int scheduledShiftId, int fromUserId, int toUserId);
    }
}