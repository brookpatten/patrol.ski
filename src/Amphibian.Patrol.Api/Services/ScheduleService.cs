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


        public async Task ApproveShiftSwap(int scheduledShiftAssignmentId)
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

        public async Task<ScheduledShift> ScheduleShift(ScheduledShiftUpdateDto dto)
        {
            var patrol = await _patrolRepository.GetPatrol(dto.PatrolId);
            TimeZoneInfo timeZone;
            if(!string.IsNullOrEmpty(patrol.TimeZone))
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(patrol.TimeZone);
            }
            else
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }

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
                var shifts = await _shiftRepository.GetShifts(patrol.Id, localizedStart.Hour, localizedStart.Minute, localizedEnd.Hour, localizedEnd.Minute);
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
                var existing = await _shiftRepository.GetScheduledShifts(patrol.Id, dto.StartsAt.Value, dto.EndsAt.Value);
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
                        PatrolId = patrol.Id,
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
    }
}
