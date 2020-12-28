using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dommel;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using System.Data.Common;

namespace Amphibian.Patrol.Api.Repositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly DbConnection _connection;

        public WorkItemRepository(DbConnection connection)
        {
            _connection = connection;
        }
        public Task<RecurringWorkItem> GetRecurringWorkItem(int id)
        {
            return _connection.GetAsync<RecurringWorkItem>(id);
        }

        public Task<ShiftRecurringWorkItem> GetShiftRecurringWorkItem(int id)
        {
            return _connection.GetAsync<ShiftRecurringWorkItem>(id);
        }

        public Task<WorkItem> GetWorkItem(int id)
        {
            return _connection.GetAsync<WorkItem>(id);
        }

        public Task<WorkItemAssignment> GetWorkItemAssignment(int id)
        {
            return _connection.GetAsync<WorkItemAssignment>(id);
        }

        public async Task InsertRecurringWorkItem(RecurringWorkItem item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public async Task InsertShiftRecurringWorkItem(ShiftRecurringWorkItem item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public async Task InsertWorkItem(WorkItem item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public async Task InsertWorkItemAssignment(WorkItemAssignment item)
        {
            int id = (int)await _connection.InsertAsync(item);
            item.Id = id;
        }

        public Task UpdateRecurringWorkItem(RecurringWorkItem item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task UpdateShiftRecurringWorkItem(ShiftRecurringWorkItem item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task UpdateWorkItem(WorkItem item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task UpdateWorkItemAssignment(WorkItemAssignment item)
        {
            return _connection.UpdateAsync(item);
        }

        public Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignments(int workItemId)
        {
            return _connection.SelectAsync<WorkItemAssignment>(x => x.WorkItemId == workItemId);
        }

        public Task<IEnumerable<ShiftRecurringWorkItem>> GetShiftRecurringWorkItems(int recurringWorkItemId)
        {
            return _connection.SelectAsync<ShiftRecurringWorkItem>(x => x.RecurringWorkItemId == recurringWorkItemId);
        }

        public Task DeleteShiftRecurringWorkItem(ShiftRecurringWorkItem shiftRecurringWorkItem)
        {
            return _connection.DeleteAsync(shiftRecurringWorkItem);
        }
        public Task<IEnumerable<WorkItem>> GetWorkItems(int recurringWorkItemId,DateTime after)
        {
            return _connection.QueryAsync<WorkItem>(@"select wi.*
                from workitems wi 
                where wi.recurringworkitemid = @recurringWorkItemId and wi.scheduledat > @after", new { recurringWorkItemId, after });
        }

        public Task DeleteWorkItem(WorkItem item)
        {
            return _connection.DeleteAsync(item);
        }

        public Task DeleteWorkItemAssignment(WorkItemAssignment item)
        {
            return _connection.DeleteAsync(item);
        }

        public Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignments(int recurringWorkItemId, DateTime after)
        {
            return _connection.QueryAsync<WorkItemAssignment>(@"select distinct wia.*
                from workitems wi 
                inner join workitemassignments wia on wia.workitemid=wi.id
                where wi.recurringworkitemid = @recurringWorkItemId and wi.scheduledat > @after", new { recurringWorkItemId, after });
        }

        public Task<IEnumerable<WorkItem>> GetWorkItemsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<WorkItem>(@"select * from workitems where scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<WorkItemAssignment>> GetWorkItemAssignmentsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<WorkItemAssignment>(@"select wia.*
                from workitems wi 
                inner join workitemassignments wia on wia.workitemid=wi.id
                where wi.scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<RecurringWorkItem>> GetRecurringWorkItemsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<RecurringWorkItem>(@"select distinct rwi.*
                from workitems wi 
                inner join recurringworkitems rwi on rwi.id=wi.recurringworkitemid
                where wi.scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }

        public Task<IEnumerable<ShiftRecurringWorkItem>> GetShiftRecurringWorkItemsForShifts(IList<int> scheduledShiftIds)
        {
            return _connection.QueryAsync<ShiftRecurringWorkItem>(@"select distinct srwi.*
                from workitems wi 
                inner join recurringworkitems rwi on rwi.id=wi.recurringworkitemid
                inner join shiftrecurringworkitems srwi on srwi.recurringworkitemid=rwi.id
                where wi.scheduledshiftid in @scheduledShiftIds", new { scheduledShiftIds });
        }
    }
}
