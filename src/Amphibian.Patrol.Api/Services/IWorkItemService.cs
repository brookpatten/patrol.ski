using Amphibian.Patrol.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IWorkItemService
    {
        Task CreateRecurringWorkItem(RecurringWorkItemDto recurringWorkItem);
        Task UpdateRecurringWorkItem(RecurringWorkItemDto recurringWorkItem);
        Task<RecurringWorkItemDto> GetRecurringWorkItem(int id);
        Task CreateWorkItem(WorkItemDto workItem);
        Task UpdateWorkItem(WorkItemDto workItem);
        Task<WorkItemDto> GetWorkItem(int id);
        Task CompleteWorkItem(int workItemId, int userId, string workNotes);

        //get work items for user
        //get assignments for work item
        //get shifts for recurring work item
    }
}