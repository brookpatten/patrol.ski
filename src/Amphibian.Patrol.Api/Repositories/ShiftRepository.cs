using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dommel;
using AutoMapper;

using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System.Data.Common;
using Amphibian.Patrol.Api.Extensions;

namespace Amphibian.Patrol.Api.Repositories
{
    public class ShiftRepository: IShiftRepository
    {
        private readonly DbConnection _connection;
        
        public ShiftRepository(DbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<ScheduledShiftAssignmentDto>> GetAvailableTrainerShiftsForTrainee(int patrolId, int traineeUserId, DateTime after)
        {
            return await _connection.QueryAsync<ScheduledShiftAssignmentDto,UserIdentifier, UserIdentifier, UserIdentifier, Group, Shift, ScheduledShiftAssignmentDto>(
                @"select distinct
                    st.id
                    ,st.scheduledshiftid
                    ,ts.StartsAt
                    ,ts.EndsAt
                    , (select count(te.id) from trainees te where te.scheduledshiftassignmentid=st.id) traineecount
                    ,st.status

                    ,au.id
                    ,au.Email
                    ,au.firstname
                    ,au.LastName

                    ,cbu.id
                    ,cbu.Email
                    ,cbu.firstname
                    ,cbu.LastName

                    ,ou.id
                    ,ou.Email
                    ,ou.firstname
                    ,ou.LastName
                
                    ,ssag.id
                    ,ssag.name

                    ,shifts.id
                    ,shifts.name

                    from scheduledshiftassignments st
                    inner join scheduledshifts ts on 
	                    ts.Id = st.scheduledshiftid
	                    and ts.PatrolId = @patrolId
	                    and ts.StartsAt > @after
                    left join users au on 
	                    au.id=st.assigneduserid
                    left join users cbu on 
	                    cbu.id=st.claimedbyuserid
                    left join users ou on 
	                    ou.id=st.originalassigneduserid
                    --join assignments to exclude trainers who cannot help
                    inner join assignments asmnts on 
	                    asmnts.userid=@traineeUserId
	                    and asmnts.CompletedAt is null
                    inner join plans p on 
	                    p.id=asmnts.PlanId
                        and p.patrolId = @patrolId
                    inner join plansections ps on
	                    ps.planid=p.id
                    inner join sections s on 
	                    s.id=ps.SectionId
                    inner join sectiongroups sg on 
	                    sg.SectionId=s.id
                    inner join groups g on 
	                    g.id=sg.groupid
                    inner join groupusers gu on 
	                    gu.GroupId=g.id
	                    and gu.UserId = st.assigneduserid
                    --join signatures to exclude things already signed
                    left join sectionskills ss on
	                    ss.SectionId = s.id
                    left join sectionlevels sl on
	                    sl.SectionId = s.Id
                    left join signatures sig on
	                    sig.AssignmentId = asmnts.id
	                    and sig.sectionskillid=ss.id
	                    and sig.sectionlevelid=sl.id
                    --join trainees to exclude shifts/trainers already signed up for
                    left join trainees t on 
	                    t.scheduledshiftassignmentid = st.id
	                    and t.TraineeUserId = @traineeUserId
                    left join groups ssag on ssag.id=ts.groupid
                    left join shifts on shifts.id=ts.shiftid
                    where 
	                    t.id is null 
	                    and sig.id is null
                    order by
	                    ts.startsat, au.lastname, au.firstname",
                (st,au,cbu,ou,g,s)=>
                {
                    st.AssignedUser = au;
                    st.ClaimedByUser = cbu;
                    st.OriginalAssignedUser = ou;
                    st.Group = g;
                    st.Shift = s;
                    return st;
                }
                , new { patrolId, traineeUserId, after });
        }
        public async Task<IEnumerable<ScheduledShiftAssignmentDto>> GetCommittedTrainerShiftsForTrainee(int patrolId, int traineeUserId, DateTime after)
        {
            return await _connection.QueryAsync<ScheduledShiftAssignmentDto, UserIdentifier, UserIdentifier, UserIdentifier, Group, Shift, ScheduledShiftAssignmentDto>(
                @"select
                st.id
                , st.scheduledshiftid
                , ts.StartsAt
                , ts.EndsAt
                , t.id traineeid
                , (select count(te.id) from trainees te where te.scheduledshiftassignmentid=st.id) traineecount
                ,st.status
                ,au.id
                ,au.Email
                ,au.firstname
                ,au.LastName

                ,cbu.id
                ,cbu.Email
                ,cbu.firstname
                ,cbu.LastName

                ,ou.id
                ,ou.Email
                ,ou.firstname
                ,ou.LastName
                
                ,g.id
                ,g.name

                ,s.id
                ,s.name
                from 
                    scheduledshiftassignments st
                inner join trainees t on
                    t.scheduledshiftassignmentid = st.id
                    and t.TraineeUserId = @traineeUserId
                inner join scheduledshifts ts on
                    ts.Id = st.scheduledshiftid
                    and ts.PatrolId = @patrolId
                    and ts.StartsAt > @after
                left join users au on 
	                au.id=st.assigneduserid
                left join users cbu on 
	                cbu.id=st.claimedbyuserid
                left join users ou on 
	                ou.id=st.originalassigneduserid
                left join groups g on g.id=ts.groupid
                left join shifts s on s.id=ts.shiftid
                order by
	                    ts.startsat",
                (st, au, cbu, ou,g,s) =>
                {
                    st.AssignedUser = au;
                    st.ClaimedByUser = cbu;
                    st.OriginalAssignedUser = ou;
                    st.Group = g;
                    st.Shift = s;
                    return st;
                }, new { patrolId, traineeUserId, after });
        }
        public async Task<IEnumerable<ScheduledShiftAssignmentDto>> GetScheduledShiftAssignments(int patrolId, int? userId=null, DateTime? from = null, DateTime? to = null, ShiftStatus? status=null, int? scheduledShiftId=null, int? noOverlapWithExistingScheduleUserId = null, int? shiftId = null)
        {
            return await _connection.QueryAsync<ScheduledShiftAssignmentDto, UserIdentifier, UserIdentifier, UserIdentifier,Group,Shift, ScheduledShiftAssignmentDto>(
                @"select
                distinct
                st.id
                , st.scheduledshiftid
                , ts.StartsAt
                , ts.EndsAt
                , (select count(te.id) from trainees te where te.scheduledshiftassignmentid=st.id) traineecount
                , st.status
                ,au.id
                ,au.Email
                ,au.firstname
                ,au.LastName

                ,cbu.id
                ,cbu.Email
                ,cbu.firstname
                ,cbu.LastName

                ,ou.id
                ,ou.Email
                ,ou.firstname
                ,ou.LastName
                
                ,g.id
                ,g.name

                ,s.id
                ,s.name

                from 
                    scheduledshiftassignments st
                inner join scheduledshifts ts on
                    ts.Id = st.scheduledshiftid
                    and ts.PatrolId = @patrolId
                    
                    
                    and (@from is null or ts.StartsAt > @from or ts.endsat > @from)
                    and (@to is null or ts.startsAt < @to or ts.endsat < @to)
                left join users au on 
	                au.id=st.assigneduserid
                left join users cbu on 
	                cbu.id=st.claimedbyuserid
                left join users ou on 
	                ou.id=st.originalassigneduserid

                left join groups g on g.id=ts.groupid

                left join shifts s on s.id=ts.shiftid

                where
                    ((@userId is not null and 
                    (st.assigneduserid = @userId
                    or st.claimedbyuserid = @userId))
                    or @userId is null)
                    and (@status is null or @status=st.status)
                    and (@scheduledShiftId is null or ts.id=@scheduledShiftId)
                    and (@shiftId is null or ts.shiftId = @shiftId)
                    --exclude shifts with overlaps
                    and (@noOverlapWithExistingScheduleUserId is null or 
                    (
		                select count(overlapssa.id) from ScheduledShiftAssignments overlapssa
		                inner join scheduledshifts overlapss on 
			                overlapss.id=overlapssa.ScheduledShiftId 
			                and overlapss.PatrolId=@patrolId
			                and overlapss.startsat <= ts.endsat
			                and overlapss.endsat >= ts.startsat
		                where 
			                overlapssa.AssignedUserId = @noOverlapWithExistingScheduleUserId
			                or overlapssa.ClaimedByUserId = @noOverlapWithExistingScheduleUserId
	                )=0)
                order by
	                    ts.startsat",
                (st, au, cbu, ou,g,s) =>
                {
                    st.AssignedUser = au;
                    st.ClaimedByUser = cbu;
                    st.OriginalAssignedUser = ou;
                    st.Group = g;
                    st.Shift = s;
                    return st;
                }, new { patrolId, userId, from,to,status, scheduledShiftId, noOverlapWithExistingScheduleUserId, shiftId });
        }
        public async Task<Trainee> InsertTrainee(Trainee trainee)
        {
            trainee.Id = (int)await _connection.InsertAsync(trainee).ConfigureAwait(false).ToInt32();
            return trainee;
        }
        public Task<IEnumerable<Trainee>> GetTrainees(int scheduledShiftAssignmentId)
        {
            return _connection.SelectAsync<Trainee>(x => x.ScheduledShiftAssignmentId == scheduledShiftAssignmentId);
        }

        public async Task<Trainee> GetTrainee(int id)
        {
            return await _connection.GetAsync<Trainee>(id);
        }

        public async Task DeleteTrainee(Trainee trainee)
        {
            await _connection.DeleteAsync<Trainee>(trainee);
        }

        public async Task<ScheduledShiftAssignment> GetScheduledShiftAssignment(int id)
        {
            return await _connection.GetAsync<ScheduledShiftAssignment>(id);
        }

        public async Task<ScheduledShift> GetScheduledShift(int id)
        {
            return await _connection.GetAsync<ScheduledShift>(id);
        }

        public Task<IEnumerable<ScheduledShift>> GetScheduledShifts(int patrolId, DateTime startsAt,DateTime endsAt)
        {
            return _connection.QueryAsync<ScheduledShift>(@"
            select
            s.*
            from scheduledshifts s
            where 
            s.patrolId=@patrolId
            and s.startsAt = @startsAt
            and s.endsAt = @endsAt
            ", new { patrolId, startsAt, endsAt });
        }

        public Task UpdateScheduledShift(ScheduledShift shift)
        {
            return _connection.UpdateAsync(shift);
        }
        public async Task InsertScheduledShift(ScheduledShift shift)
        {
            var id = (int)await _connection.InsertAsync(shift).ConfigureAwait(false).ToInt32();
            shift.Id = id;
        }
        public Task DeleteScheduledShift(ScheduledShift shift)
        {
            return _connection.DeleteAsync(shift);
        }

        public Task<IEnumerable<Shift>> GetShifts(int patrolId, int? startHour = null, int? startMinute = null, int? endHour = null, int? endMinute = null)
        {
            return _connection.QueryAsync<Shift>(@"
            select
            s.*
            from shifts s
            where 
            s.patrolId=@patrolId
            and (@startHour is null or s.startHour=@startHour)
            and (@startMinute is null or s.startMinute=@startMinute)
            and (@endHour is null or s.endHour=@endHour)
            and (@endMinute is null or s.endMinute=@endMinute)
            order by s.startHour,s.startMinute,s.Name
            ", new { patrolId, startHour, startMinute, endHour, endMinute });
        }
        public Task<Shift> GetShift(int id)
        {
            return _connection.GetAsync<Shift>(id);
        }
        public async Task InsertShift(Shift shift)
        {
            var id = (int)await _connection.InsertAsync(shift).ConfigureAwait(false).ToInt32();
            shift.Id = id;
        }
        public Task UpdateShift(Shift shift)
        {
            return _connection.UpdateAsync(shift);
        }
        public Task DeleteShift(Shift shift)
        {
            return _connection.DeleteAsync(shift);
        }

        public async Task InsertScheduledShiftAssignment(ScheduledShiftAssignment shift)
        {
            var id = (int)await _connection.InsertAsync(shift).ConfigureAwait(false).ToInt32();
            shift.Id = id;
        }
        public Task UpdateScheduledShiftAssignment(ScheduledShiftAssignment shift)
        {
            return _connection.UpdateAsync(shift);
        }
        public Task DeleteScheduledShiftAssignment(ScheduledShiftAssignment shift)
        {
            return _connection.DeleteAsync(shift);
        }
        public Task<IEnumerable<ScheduledShiftAssignment>> GetScheduledShiftAssignmentsForScheduledShift(int scheduledShiftId)
        {
            return _connection.SelectAsync<ScheduledShiftAssignment>(x => x.ScheduledShiftId == scheduledShiftId);
        }
    }
}
