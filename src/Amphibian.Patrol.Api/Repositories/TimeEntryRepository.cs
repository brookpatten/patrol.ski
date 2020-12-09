using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dommel;
using Amphibian.Patrol.Api.Dtos;

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
            from timeentrys
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

        public Task<IEnumerable<CurrentTimeEntryDto>> GetActiveTimeEntries(int patrolId, DateTime now)
        {
            return _connection.QueryAsync<User, TimeEntry, ScheduledShift, TimeEntryScheduledShiftAssignment, Shift, Group, CurrentTimeEntryDto>(@"
            select 
            u.id
            ,u.firstname
            ,u.lastname
            ,u.email
            ,te.id
            ,te.clockin
            ,te.clockout
            ,te.durationseconds
            ,ss.id
            ,ss.startsat
            ,ss.endsat
            ,ss.durationseconds
            ,tessa.id
            ,tessa.durationseconds
            ,s.id
            ,s.name
            ,g.id
            ,g.name
            from timeentrys te
            inner join users u on u.id=te.userid
            left join timeentryscheduledshiftassignments tessa on tessa.timeentryid=te.id
            left join scheduledshiftassignments ssa on ssa.id=tessa.scheduledshiftassignmentid
            left join scheduledshifts ss on ss.id=ssa.scheduledshiftid
            left join shifts s on s.id=ss.shiftid
            left join groups g on g.id=ss.groupid
            where 
                te.patrolid=@patrolId
                and te.clockout is null
                and te.clockin <= @now
            order by u.lastname,u.firstname
            "
            ,(u,te,ss,tessa,s,g)=> {
                var dto = new CurrentTimeEntryDto();
                dto.User = u;
                dto.TimeEntry = te;
                dto.ScheduledShift = ss;
                dto.TimeEntryScheduledShiftAssignment = tessa;
                dto.Shift = s;
                dto.Group = g;
                return dto;
            } ,new { patrolId,now });
        }
        public Task<IEnumerable<CurrentTimeEntryDto>> GetMissingTimeEntries(int patrolId, DateTime now)
        {
            return _connection.QueryAsync<User, TimeEntry, ScheduledShift, Shift, Group, CurrentTimeEntryDto>(@"
            select 
            u.id
            ,u.firstname
            ,u.lastname
            ,u.email
            ,te.id
            ,te.clockin
            ,te.clockout
            ,te.durationseconds
            ,ss.id
            ,ss.startsat
            ,ss.endsat
            ,ss.durationseconds
            ,s.id
            ,s.name
            ,g.id
            ,g.name
            from scheduledshifts ss
            inner join scheduledshiftassignments ssa on ssa.scheduledshiftid=ss.id
            inner join users u on u.id=ssa.assigneduserid
            left join timeentrys te on 
                te.userid=ssa.assigneduserid 
                and te.patrolid=@patrolid 
                and te.clockin<@now 
                and te.clockout is null
            left join shifts s on s.id=ss.shiftid
            left join groups g on g.id=ss.groupid
            where 
                ss.patrolid=@patrolId
                and ss.startsat <= @now 
                and ss.endsat >= @now
                and te.id is null
            order by u.lastname,u.firstname
            "
            , (u, te, ss, s, g) => {
                var dto = new CurrentTimeEntryDto();
                dto.User = u;
                dto.TimeEntry = te;
                dto.ScheduledShift = ss;
                dto.Shift = s;
                dto.Group = g;
                return dto;
            }, new { patrolId, now });
        }
        public Task<IEnumerable<CurrentTimeEntryDto>> GetTimeEntries(int patrolId, int? userId, DateTime from, DateTime to)
        {
            return _connection.QueryAsync<User, TimeEntry, ScheduledShift, TimeEntryScheduledShiftAssignment, Shift, Group, CurrentTimeEntryDto>(@"
            select 
            u.id
            ,u.firstname
            ,u.lastname
            ,u.email
            ,te.id
            ,te.clockin
            ,te.clockout
            ,te.durationseconds
            ,ss.id
            ,ss.startsat
            ,ss.endsat
            ,ss.durationseconds
            ,tessa.id
            ,tessa.durationseconds
            ,s.id
            ,s.name
            ,g.id
            ,g.name
            from timeentrys te
            inner join users u on u.id=te.userid
            left join timeentryscheduledshiftassignments tessa on tessa.timeentryid=te.id
            left join scheduledshiftassignments ssa on ssa.id=tessa.scheduledshiftassignmentid
            left join scheduledshifts ss on ss.id=ssa.scheduledshiftid
            left join shifts s on s.id=ss.shiftid
            left join groups g on g.id=ss.groupid
            where 
                te.patrolid=@patrolId
                and te.clockout >= @from 
                and te.clockin <= @to
                and (@userId is null or te.userid=@userId)
            order by te.clockin desc
            "
            , (u, te, ss,tessa, s, g) => {
                var dto = new CurrentTimeEntryDto();
                dto.User = u;
                dto.TimeEntry = te;
                dto.ScheduledShift = ss;
                dto.TimeEntryScheduledShiftAssignment = tessa;
                dto.Shift = s;
                dto.Group = g;
                return dto;
            }, new { patrolId, userId, from,to });
        }

        public Task<IEnumerable<CurrentTimeEntryDto>> GetMissingShiftTime(int patrolId, int? userId, DateTime from, DateTime to)
        {
            return _connection.QueryAsync<User, TimeEntry, ScheduledShift, TimeEntryScheduledShiftAssignment, Shift, Group, CurrentTimeEntryDto>(@"
            select
	            u.id
	            ,u.firstname
	            ,u.lastname
	            ,0 id --has to be non null or dommel will null out the entire object
	            ,min(te.clockin) clockin
	            ,max(te.clockout) clockout
	            ,ss.id
	            ,ss.startsat
	            ,ss.endsat
	            ,ss.durationseconds
	            ,0 id --has to be non null or dommel will null out the entire object
	            ,sum(tessa.durationseconds) durationseconds
	            ,s.id
	            ,s.name
	            ,g.id
	            ,g.name
            from scheduledshifts ss
            inner join scheduledshiftassignments ssa on
	            ssa.scheduledshiftid=ss.id
            inner join users u on u.id=ssa.assigneduserid
            left join timeentryscheduledshiftassignments tessa on
	            tessa.scheduledshiftassignmentid=ssa.id
            left join timeentrys te on te.id=tessa.timeentryid
            left join shifts s on s.id=ss.shiftid
            left join groups g on g.id=ss.groupid
            where 
                ss.startsat <= @to
                and ss.endsat >= @from
                and ss.patrolId = @patrolId
                and (@userId is null or u.id=@userId)
            group by 
	            u.id
	            ,u.firstname
	            ,u.lastname
	            ,ss.id
	            ,ss.startsat
	            ,ss.endsat
	            ,ss.durationseconds
	            ,s.id
	            ,s.name
	            ,g.id
	            ,g.name
            having sum(tessa.durationseconds) < ss.durationseconds or sum(tessa.durationseconds) is null
            order by ss.startsat desc
            "
            , (u, te, ss, tessa, s, g) => {
                var dto = new CurrentTimeEntryDto();
                dto.User = u;
                dto.TimeEntry = te;
                dto.ScheduledShift = ss;
                dto.TimeEntryScheduledShiftAssignment = tessa;
                dto.Shift = s;
                dto.Group = g;
                return dto;
            }, new { patrolId, userId, from, to });
        }
    }
}
