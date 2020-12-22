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

        Task<RecurringWorkItem> GetRecurringWorkItem(int id);
        Task InsertRecurringWorkItem(RecurringWorkItem item);
        Task UpdateRecurringWorkItem(RecurringWorkItem item);

        Task<WorkItemAssignment> GetWorkItemAssignment(int id);
        Task InsertWorkItemAssignment(WorkItemAssignment item);
        Task UpdateWorkItemAssignment(WorkItemAssignment item);

        Task<ShiftRecurringWorkItem> GetShiftRecurringWorkItem(int id);
        Task InsertShiftRecurringWorkItem(ShiftRecurringWorkItem item);
        Task UpdateShiftRecurringWorkItem(ShiftRecurringWorkItem item);
    }
}
