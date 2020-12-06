using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dommel;

namespace Amphibian.Patrol.Api.Repositories
{
    public class TimeEntryRepository : ITimeEntryRepository
    {
        private readonly DbConnection _connection;
        public TimeEntryRepository(DbConnection connection)
        {
            _connection = connection;
        }
        public Task DeleteTimeEntry(TimeEntry entry)
        {
            return _connection.DeleteAsync(entry);
        }

        public Task DeleteTimeEntryScheduledShiftAssignment(TimeEntryScheduledShiftAssignment timeEntryScheduledShiftAssignment)
        {
            return _connection.DeleteAsync(timeEntryScheduledShiftAssignment);
        }

        public Task<IEnumerable<TimeEntry>> GetActiveTimeEntries(int? patrolId,int? userId)
        {
            return _connection.QueryAsync<TimeEntry>(@"
            select 
            *
            from timeentries
            where clockout is null
            and (
                @patrolId is null
                or patrolId=@patrolId
            )
            and (
                @userId is null
                or userId=@userId
            )
            order by clockin asc
            ", new { patrolId,userId });
        }

        public Task<IEnumerable<TimeEntryScheduledShiftAssignment>> GetScheduledShiftAssignmentsForTimeEntry(int timeEntryId)
        {
            return _connection.SelectAsync<TimeEntryScheduledShiftAssignment>(x => x.TimeEntryId == timeEntryId);
        }

        public Task<IEnumerable<TimeEntryScheduledShiftAssignment>> GetScheduledShiftAssignmentsForScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            return _connection.SelectAsync<TimeEntryScheduledShiftAssignment>(x => x.ScheduledShiftAssignmentId == scheduledShiftAssignmentId);
        }

        public Task<TimeEntry> GetTimeEntry(int id)
        {
            return _connection.GetAsync<TimeEntry>(id);
        }

        public Task<TimeEntryScheduledShiftAssignment> GetTimeEntryScheduledShiftAssignment(int id)
        {
            return _connection.GetAsync<TimeEntryScheduledShiftAssignment>(id);
        }

        public async Task InsertTimeEntry(TimeEntry entry)
        {
            var id = (int)(await _connection.InsertAsync(entry));
            entry.Id = id;
        }

        public async Task InsertTimeEntryScheduledShiftAssignment(TimeEntryScheduledShiftAssignment timeEntryScheduledShiftAssignment)
        {
            var id = (int)(await _connection.InsertAsync(timeEntryScheduledShiftAssignment));
            timeEntryScheduledShiftAssignment.Id = id;
        }

        public Task UpdateTimeEntry(TimeEntry entry)
        {
            return _connection.UpdateAsync(entry);
        }

        public Task UpdateTimeEntryScheduledShiftAssignment(TimeEntryScheduledShiftAssignment timeEntryScheduledShiftAssignment)
        {
            return _connection.UpdateAsync(timeEntryScheduledShiftAssignment);
        }
    }
}
