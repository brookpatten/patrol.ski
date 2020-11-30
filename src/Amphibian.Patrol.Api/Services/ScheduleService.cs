using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public class ScheduleService : IScheduleService
    {
        private ILogger<ScheduleService> _logger;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        private IShiftRepository _shiftRepository;
        private ISystemClock _clock;

        public ScheduleService(ILogger<ScheduleService> logger, IPatrolRepository patrolRepository, 
            IGroupRepository groupRepository,IShiftRepository shiftRepository, ISystemClock clock)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
            _shiftRepository = shiftRepository;
            _clock = clock;
        }


        public async Task ApproveShiftSwap(int scheduledShiftAssignmentId, int userId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Claimed && ssa.ClaimedByUserId.HasValue)
            {
                ssa.Status = ShiftStatus.Assigned;
                ssa.AssignedUserId = ssa.ClaimedByUserId.Value;
                ssa.ClaimedByUserId = null;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for approval");
            }
        }

        public async Task DeclineShiftSwap(int scheduledShiftAssignmentId, int userId, string reason)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Claimed && ssa.ClaimedByUserId.HasValue)
            {
                ssa.Status = ShiftStatus.Assigned;
                ssa.ClaimedByUserId = null;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for decline");
            }
        }

        public async Task CancelShift(int scheduledShiftId)
        {
            var scheduledShift = await _shiftRepository.GetScheduledShift(scheduledShiftId);
            var scheduledShiftAssignments = await _shiftRepository.GetScheduledShiftAssignmentsForScheduledShift(scheduledShiftId);

            foreach(var ssa in scheduledShiftAssignments)
            {
                await _shiftRepository.DeleteScheduledShiftAssignment(ssa);
            }
            await _shiftRepository.DeleteScheduledShift(scheduledShift);
        }

        public async Task ClaimShift(int scheduledShiftAssignmentId, int userId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Released)
            {
                ssa.Status = ShiftStatus.Claimed;
                ssa.ClaimedByUserId = userId;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for claim");
            }
        }

        public async Task ReleaseShift(int scheduledShiftAssignmentId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Assigned)
            {
                ssa.Status = ShiftStatus.Released;
                ssa.ClaimedByUserId = null;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);
            }
            else if(ssa.Status == ShiftStatus.Released)
            {
                //do nothing
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for release");
            }
        }

        public async Task CancelShiftRelease(int scheduledShiftAssignmentId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Released || ssa.Status == ShiftStatus.Claimed)
            {
                ssa.Status = ShiftStatus.Assigned;
                ssa.ClaimedByUserId = null;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for cancel release");
            }
        }

        private async Task<TimeZoneInfo> GetTimeZoneForPatrolId(int patrolId)
        {
            var patrol = await _patrolRepository.GetPatrol(patrolId);
            TimeZoneInfo timeZone;
            if (!string.IsNullOrEmpty(patrol.TimeZone))
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(patrol.TimeZone);
            }
            else
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            return timeZone;
        }

        public async Task<ScheduledShift> ScheduleShift(ScheduledShiftUpdateDto dto)
        {
            TimeZoneInfo timeZone = await GetTimeZoneForPatrolId(dto.PatrolId);
            
            Group group = null;
            IList<User> groupMembers = null;
            if(dto.GroupId.HasValue)
            {
                group = await _groupRepository.GetGroup(dto.GroupId.Value);
                groupMembers = (await _groupRepository.GetUsersInGroup(dto.GroupId.Value)).ToList();
            }

            Shift shift = null;
            if(dto.StartsAt.HasValue && dto.EndsAt.HasValue && !dto.ShiftId.HasValue)
            {
                var localizedStart = TimeZoneInfo.ConvertTimeFromUtc(dto.StartsAt.Value, timeZone);
                var localizedEnd = TimeZoneInfo.ConvertTimeFromUtc(dto.EndsAt.Value, timeZone);
                var shifts = await _shiftRepository.GetShifts(dto.PatrolId, localizedStart.Hour, localizedStart.Minute, localizedEnd.Hour, localizedEnd.Minute);
                if(shifts.Any())
                {
                    shift = shifts.First();
                    dto.ShiftId = shift.Id;
                }
            }
            else if(dto.ShiftId.HasValue && dto.Day.HasValue)
            {
                shift = await _shiftRepository.GetShift(dto.ShiftId.Value);

                if(!dto.StartsAt.HasValue)
                {
                    var start = new DateTime(dto.Day.Value.Year, dto.Day.Value.Month, dto.Day.Value.Day, shift.StartHour, shift.StartMinute, 0, 0, DateTimeKind.Unspecified);
                    dto.StartsAt = TimeZoneInfo.ConvertTimeToUtc(start, timeZone);
                }
                if(!dto.EndsAt.HasValue)
                {
                    var end = new DateTime(dto.Day.Value.Year, dto.Day.Value.Month, dto.Day.Value.Day, shift.EndHour, shift.EndMinute, 0, 0, DateTimeKind.Unspecified);
                    dto.EndsAt = TimeZoneInfo.ConvertTimeToUtc(end, timeZone);
                }
            }
            else
            {
                throw new InvalidOperationException("Scheduled shift must provide either start+end or day+shift");
            }

            //by this point we should have filled in the dto as best we can and can now persist

            ScheduledShift scheduledShift = null;
            List<ScheduledShiftAssignment> assignments = null;
            if(dto.Id!=default(int))
            {
                scheduledShift = await _shiftRepository.GetScheduledShift(dto.Id);

                scheduledShift.StartsAt = dto.StartsAt.Value;
                scheduledShift.EndsAt = dto.EndsAt.Value;
                scheduledShift.GroupId = group != null ? (int?)group.Id : null;
                scheduledShift.ShiftId = shift != null ? (int?)shift.Id : null;
                await _shiftRepository.UpdateScheduledShift(scheduledShift);

                assignments = (await _shiftRepository.GetScheduledShiftAssignmentsForScheduledShift(dto.Id)).ToList();
            }
            else
            {
                //make sure there's not an existing shift with the same start/end
                var existing = await _shiftRepository.GetScheduledShifts(dto.PatrolId, dto.StartsAt.Value, dto.EndsAt.Value);
                if(existing.Any())
                {
                    scheduledShift = existing.First();
                    dto.Id = scheduledShift.Id;

                    bool updated = false;
                    if (!scheduledShift.GroupId.HasValue && group!=null)
                    {
                        scheduledShift.GroupId = group.Id;
                        updated = true;
                    }
                    if(!scheduledShift.ShiftId.HasValue && shift!=null)
                    {
                        scheduledShift.ShiftId = shift.Id;
                        updated = true;
                    }
                    if (updated)
                    {
                        await _shiftRepository.UpdateScheduledShift(scheduledShift);
                    }

                    assignments = (await _shiftRepository.GetScheduledShiftAssignmentsForScheduledShift(dto.Id)).ToList();
                }
                else
                {
                    scheduledShift = new ScheduledShift()
                    {
                        PatrolId = dto.PatrolId,
                        GroupId = group !=null ? (int?)group.Id : null,
                        ShiftId = shift!=null ? (int?)shift.Id : null,
                        StartsAt = dto.StartsAt.Value,
                        EndsAt = dto.EndsAt.Value
                    };
                    await _shiftRepository.InsertScheduledShift(scheduledShift);
                    assignments = new List<ScheduledShiftAssignment>();
                }
            }

            if(group!=null)
            {
                foreach(var groupMember in groupMembers)
                {
                    var existing = assignments.FirstOrDefault(x => x.OriginalAssignedUserId == groupMember.Id || x.AssignedUserId == groupMember.Id || x.ClaimedByUserId == groupMember.Id);
                    if(existing==null)
                    {
                        var ssa = new ScheduledShiftAssignment()
                        {
                            Status = ShiftStatus.Assigned,
                            AssignedUserId = groupMember.Id,
                            OriginalAssignedUserId = groupMember.Id,
                            ScheduledShiftId = scheduledShift.Id
                        };
                        assignments.Add(ssa);
                        await _shiftRepository.InsertScheduledShiftAssignment(ssa);
                    }
                    else if (existing.Status == ShiftStatus.Claimed && existing.ClaimedByUserId == groupMember.Id)
                    {
                        existing.AssignedUserId = existing.ClaimedByUserId.Value;
                        existing.Status = ShiftStatus.Assigned;
                        await _shiftRepository.UpdateScheduledShiftAssignment(existing);
                    }
                }
            }

            if(dto.AssignUserIds!=null)
            {
                foreach (var id in dto.AssignUserIds)
                {
                    var existing = assignments.FirstOrDefault(x => x.OriginalAssignedUserId == id || x.AssignedUserId == id || x.ClaimedByUserId == id);
                    if (existing == null)
                    {
                        var ssa = new ScheduledShiftAssignment()
                        {
                            Status = ShiftStatus.Assigned,
                            AssignedUserId = id,
                            OriginalAssignedUserId = id,
                            ScheduledShiftId = scheduledShift.Id
                        };
                        assignments.Add(ssa);
                        await _shiftRepository.InsertScheduledShiftAssignment(ssa);
                    }
                    else if(existing.Status == ShiftStatus.Claimed && existing.ClaimedByUserId == id)
                    {
                        existing.AssignedUserId = existing.ClaimedByUserId.Value;
                        existing.Status = ShiftStatus.Assigned;
                        await _shiftRepository.UpdateScheduledShiftAssignment(existing);
                    }
                }
            }

            //todo: delete things that aren't there?  seems risky if we found the shift by time not id etc   

            return await _shiftRepository.GetScheduledShift(scheduledShift.Id);
        }

        public async Task ReplicatePeriod(int patrolId, bool clearTargetPeriodFirst, DateTime replicatedPeriodStart, DateTime replicatedPeriodEnd, DateTime targetPeriodStart, DateTime targetPeriodEnd)
        {
            if(replicatedPeriodEnd<replicatedPeriodStart)
            {
                throw new InvalidOperationException("replicated period start must be before replicated period end");
            }
            if(targetPeriodStart < replicatedPeriodEnd && targetPeriodEnd > replicatedPeriodStart)
            {
                throw new InvalidOperationException("target period cannot intersect replicated period");
            }

            var timeZone = await GetTimeZoneForPatrolId(patrolId);

            var replicatedStartLocal = new DateTime(replicatedPeriodStart.Year, replicatedPeriodStart.Month, replicatedPeriodEnd.Day, 0, 0, 0, DateTimeKind.Unspecified);
            var replicatedStartUtc = TimeZoneInfo.ConvertTimeToUtc(replicatedStartLocal, timeZone);
            var replicatedEndLocal = new DateTime(replicatedPeriodEnd.Year, replicatedPeriodEnd.Month, replicatedPeriodEnd.Day, 23, 59, 59,999, DateTimeKind.Unspecified);
            var replicatedEndutc = TimeZoneInfo.ConvertTimeToUtc(replicatedEndLocal, timeZone);

            var targetStartLocal = new DateTime(targetPeriodStart.Year, targetPeriodStart.Month, targetPeriodStart.Day, 0, 0, 0, DateTimeKind.Unspecified);
            var targetStartUtc = TimeZoneInfo.ConvertTimeToUtc(targetStartLocal, timeZone);
            var targetEndLocal = new DateTime(targetPeriodEnd.Year, targetPeriodEnd.Month, targetPeriodEnd.Day, 23, 59, 59, 999, DateTimeKind.Unspecified);
            var targetEndUtc = TimeZoneInfo.ConvertTimeToUtc(targetEndLocal, timeZone);

            if(clearTargetPeriodFirst)
            {
                var shiftsToRemove = await _shiftRepository.GetScheduledShiftAssignments(patrolId, null, targetStartUtc, targetEndUtc, null);
                var scheduledShiftIds = shiftsToRemove.Select(x => x.ScheduledShiftId).Distinct().ToList();
                foreach(var id in scheduledShiftIds)
                {
                    await this.CancelShift(id);
                }
            }

            var shiftsToCopy = await _shiftRepository.GetScheduledShiftAssignments(patrolId, null, replicatedStartUtc, replicatedEndutc, null);
            foreach (var shift in shiftsToCopy)
            {
                shift.StartsAt = TimeZoneInfo.ConvertTimeFromUtc(shift.StartsAt, timeZone);
                shift.EndsAt = TimeZoneInfo.ConvertTimeFromUtc(shift.EndsAt, timeZone);
            }

            //group and index by day
            var grouped = shiftsToCopy.GroupBy(x => new { x.StartsAt.Year, x.StartsAt.Month, x.StartsAt.Day })
                .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month).ThenBy(x => x.Key.Day)
                .ToList();

            //step a day at a time through the target range
            for(int i=0;targetStartUtc + new TimeSpan(i,0,0,0) < targetEndUtc;i++)
            {
                var day = targetStartLocal + new TimeSpan(i, 0, 0, 0);
                var group = grouped[i % grouped.Count].GroupBy(x=>x.ScheduledShiftId);
                
                foreach(var scheduledShift in group)
                {
                    await this.ScheduleShift(new ScheduledShiftUpdateDto()
                    {
                        GroupId = scheduledShift.First().Group?.Id,
                        ShiftId = scheduledShift.First().Shift?.Id,
                        PatrolId = patrolId,
                        Day = scheduledShift.First().Shift !=null ? day : (DateTime?)null,
                        StartsAt = scheduledShift.First().Shift ==null ? TimeZoneInfo.ConvertTimeToUtc(new DateTime(day.Year,day.Month,day.Day,scheduledShift.First().StartsAt.Hour, scheduledShift.First().StartsAt.Minute,0,DateTimeKind.Unspecified),timeZone) : (DateTime?)null,
                        EndsAt = scheduledShift.First().Shift == null ? TimeZoneInfo.ConvertTimeToUtc(new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().EndsAt.Hour, scheduledShift.First().EndsAt.Minute, 0, DateTimeKind.Unspecified), timeZone) : (DateTime?)null,
                        AssignUserIds = scheduledShift.Select(x=>x.AssignedUser.Id).Distinct().ToList()
                    });
                }
            }
        }
    }
}
