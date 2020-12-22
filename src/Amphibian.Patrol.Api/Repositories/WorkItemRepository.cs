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
    }
}
