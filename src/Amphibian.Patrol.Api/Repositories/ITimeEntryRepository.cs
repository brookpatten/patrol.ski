using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface ITimeEntryRepository
    {
        Task<TimeEntry> GetTimeEntry(int id);
        Task InsertTimeEntry(TimeEntry entry);
        Task UpdateTimeEntry(TimeEntry entry);
        Task DeleteTimeEntry(TimeEntry entry);
        Task<IEnumerable<TimeEntryScheduledShiftAssignment>> GetScheduledShiftAssignmentsForTimeEntry(int timeEntryId);
        Task<IEnumerable<TimeEntryScheduledShiftAssignment>> GetScheduledShiftAssignmentsForScheduledShiftAssignment(int scheduledShiftAssignmentId);
        Task<TimeEntryScheduledShiftAssignment> GetTimeEntryScheduledShiftAssignment(int id);
        Task InsertTimeEntryScheduledShiftAssignment(TimeEntryScheduledShiftAssignment timeEntryScheduledShiftAssignment);
        Task UpdateTimeEntryScheduledShiftAssignment(TimeEntryScheduledShiftAssignment timeEntryScheduledShiftAssignment);
        Task DeleteTimeEntryScheduledShiftAssignment(TimeEntryScheduledShiftAssignment timeEntryScheduledShiftAssignment);
        Task<IEnumerable<TimeEntry>> GetActiveTimeEntries(int? patrolId, int? userId);

        Task<IEnumerable<CurrentTimeEntryDto>> GetActiveTimeEntries(int patrolId,DateTime now);
        Task<IEnumerable<CurrentTimeEntryDto>> GetMissingTimeEntries(int patrolId, DateTime now);
        Task<IEnumerable<CurrentTimeEntryDto>> GetTimeEntries(int patrolId, int? userId, DateTime from, DateTime to);
        Task<IEnumerable<CurrentTimeEntryDto>> GetMissingShiftTime(int patrolId, int? userId, DateTime from, DateTime to);
    }
}
