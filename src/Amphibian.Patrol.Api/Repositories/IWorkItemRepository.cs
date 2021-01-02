using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IWorkItemRepository
    {
        Task<WorkItem> GetWorkItem(int id);
        Task InsertWorkItem(WorkItem item);
        Task UpdateWorkItem(WorkItem item);
        Task DeleteWorkItem(WorkItem item);
        Task<IEnumerable<WorkItem>> GetWorkItems(int recurringWorkItemId, DateTime after);
        Task<IEnumerable<WorkItem>> GetIncompleteWorkItemsForUser(DateTime after, int userId, int patrolId);

        Task<RecurringWorkItem> GetRecurringWorkItem(int id);
        Task<IEnumerable<RecurringWorkItem>> GetRecurringWorkItems(int patrolId);
        Task InsertRecurringWorkItem(RecurringWorkItem item);
        Task UpdateRecurringWorkItem(RecurringWorkItem item);

        Task<WorkItemAssignment> GetWorkItemAssignment(int id);
        Task InsertWorkItemAssignment(WorkItemAssignment item);
        Task UpdateWorkItemAssignment(WorkItemAssignment item);
        Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignments(int workItemId);
        Task DeleteWorkItemAssignment(WorkItemAssignment item);
        Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignments(int recurringWorkItemId,DateTime after);

        Task<ShiftRecurringWorkItem> GetShiftRecurringWorkItem(int id);
        Task InsertShiftRecurringWorkItem(ShiftRecurringWorkItem item);
        Task UpdateShiftRecurringWorkItem(ShiftRecurringWorkItem item);
        Task<IEnumerable<ShiftRecurringWorkItem>> GetShiftRecurringWorkItems(int recurringWorkItemId);
        Task DeleteShiftRecurringWorkItem(ShiftRecurringWorkItem shiftRecurringWorkItem);

        Task<IEnumerable<WorkItem>> GetWorkItemsForShifts(IList<int> scheduledShiftIds);
        Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignmentsForShifts(IList<int> scheduledShiftIds);
        Task<IEnumerable<RecurringWorkItem>> GetRecurringWorkItemsForShifts(IList<int> scheduledShiftIds);
        Task<IEnumerable<ShiftRecurringWorkItem>> GetShiftRecurringWorkItemsForShifts(IList<int> scheduledShiftIds);
        Task<IEnumerable<WorkItemDto>> GetWorkItems(int userId, int? patrolId=null,bool? complete=null,int? completedByUserId=null,int? recurringWorkItemId=null,
            DateTime? scheduledBefore=null,DateTime? scheduledAfter=null,int? shiftId=null,int? adminGroupId=null,string name=null,
            string location=null, int? completableByUserId = null, int? workItemId = null);
    }
}
