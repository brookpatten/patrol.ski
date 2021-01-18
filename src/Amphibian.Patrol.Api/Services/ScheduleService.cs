using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Amphibian.Patrol.Api.Services
{
    public class ScheduleService : IScheduleService
    {
        private ILogger<ScheduleService> _logger;
        private IPatrolRepository _patrolRepository;
        private IGroupRepository _groupRepository;
        private IShiftRepository _shiftRepository;
        private ISystemClock _clock;
        private IEmailService _emailService;
        private IUserRepository _userRepository;
        private IShiftWorkItemService _shiftWorkItemService;

        public ScheduleService(ILogger<ScheduleService> logger, IPatrolRepository patrolRepository, 
            IGroupRepository groupRepository,IShiftRepository shiftRepository, ISystemClock clock, IEmailService emailService, IUserRepository userRepository,
            IShiftWorkItemService shiftWorkItemService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _groupRepository = groupRepository;
            _shiftRepository = shiftRepository;
            _clock = clock;
            _emailService = emailService;
            _userRepository = userRepository;
            _shiftWorkItemService = shiftWorkItemService;
        }


        public async Task ApproveShiftSwap(int scheduledShiftAssignmentId, int userId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Claimed && ssa.ClaimedByUserId.HasValue)
            {
                int? assignedUserId = ssa.AssignedUserId;
                int claimedUserId = ssa.ClaimedByUserId.Value;

                ssa.Status = ShiftStatus.Assigned;
                ssa.AssignedUserId = ssa.ClaimedByUserId.Value;
                ssa.ClaimedByUserId = null;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);

                //send notification email to assignee
                User assigned = null;
                if (assignedUserId.HasValue)
                {
                    assigned = await _userRepository.GetUser(assignedUserId.Value);
                }
                var claimed = await _userRepository.GetUser(claimedUserId);
                var approved = await _userRepository.GetUser(userId);
                var shift = await _shiftRepository.GetScheduledShift(ssa.ScheduledShiftId);
                var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);

                //also swap any relevant work items
                //TODO: make this an async event to a wi microservice?
                if (assignedUserId.HasValue)
                {
                    await _shiftWorkItemService.SwapScheduledShiftWorkItems(shift.Id, assignedUserId.Value, claimedUserId);
                }

                await _emailService.SendShiftApproved(assigned, patrol, claimed,approved, shift);
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for approval");
            }
        }

        public async Task RejectShiftSwap(int scheduledShiftAssignmentId, int userId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Claimed && ssa.ClaimedByUserId.HasValue)
            {
                int? assignedUserId = ssa.AssignedUserId;
                int claimedUserId = ssa.ClaimedByUserId.Value;

                ssa.Status = ShiftStatus.Released;
                ssa.ClaimedByUserId = null;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);

                //send notification email to assignee
                User assigned = null;
                if (assignedUserId.HasValue)
                {
                    assigned = await _userRepository.GetUser(assignedUserId.Value);
                }
                var claimed = await _userRepository.GetUser(claimedUserId);
                var rejected = await _userRepository.GetUser(userId);
                var shift = await _shiftRepository.GetScheduledShift(ssa.ScheduledShiftId);
                var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);
                await _emailService.SendShiftRejected(assigned, patrol, claimed, rejected, shift);
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
            var patrol = await _patrolRepository.GetPatrol(scheduledShift.PatrolId);


            foreach (var ssa in scheduledShiftAssignments)
            {
                await _shiftRepository.DeleteScheduledShiftAssignment(ssa);

                //send notification to any trainees that the shift has been cancelled
                var trainees = await _shiftRepository.GetTrainees(ssa.Id);
                if (trainees.Any() && ssa.AssignedUserId.HasValue)
                {
                    var trainer = await _userRepository.GetUser(ssa.AssignedUserId.Value);
                    var shift = await _shiftRepository.GetScheduledShift(ssa.ScheduledShiftId);
                    var traineeUsers = await _userRepository.GetUsers(trainees.Select(x => x.TraineeUserId).ToList());
                    await _emailService.SendTrainerShiftRemoved(trainer, traineeUsers.ToList(), patrol, shift);
                }
            }

            //remove any work items related to the shift
            await _shiftWorkItemService.RemoveWorkItemsFromShiftOccurence(scheduledShift);

            await _shiftRepository.DeleteScheduledShift(scheduledShift);

            //notify users in the shift
            var assignedUsers = await _userRepository.GetUsers(scheduledShiftAssignments.Where(x=>x.AssignedUserId.HasValue).Select(x => x.AssignedUserId.Value).ToList());
            await _emailService.SendShiftCancelled(assignedUsers.ToList(),patrol,scheduledShift);
        }

        public async Task ClaimShift(int scheduledShiftAssignmentId, int userId)
        {
            var ssa = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if (ssa.Status == ShiftStatus.Released)
            {
                ssa.Status = ShiftStatus.Claimed;
                ssa.ClaimedByUserId = userId;
                await _shiftRepository.UpdateScheduledShiftAssignment(ssa);

                //send notification email to assignee
                if (ssa.AssignedUserId.HasValue)
                {
                    var assigned = await _userRepository.GetUser(ssa.AssignedUserId.Value);
                    var claimed = await _userRepository.GetUser(ssa.ClaimedByUserId.Value);
                    var shift = await _shiftRepository.GetScheduledShift(ssa.ScheduledShiftId);
                    var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);
                    await _emailService.SendShiftClaimed(assigned, patrol, claimed, shift);
                }
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

                //send notification to any trainees that the shift has been released
                var trainees = await _shiftRepository.GetTrainees(scheduledShiftAssignmentId);
                if(trainees.Any() && ssa.AssignedUserId.HasValue)
                {
                    var trainer = await _userRepository.GetUser(ssa.AssignedUserId.Value);
                    var shift = await _shiftRepository.GetScheduledShift(ssa.ScheduledShiftId);
                    var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);
                    var traineeUsers = await _userRepository.GetUsers(trainees.Select(x => x.TraineeUserId).ToList());
                    await _emailService.SendTrainerShiftReleased(trainer, traineeUsers.ToList(), patrol, shift);
                }
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

                //TODO notify claimed if it's not null?
            }
            else
            {
                throw new InvalidOperationException($"ScheduledShiftAssignment is not in a valid state for cancel release");
            }
        }

        public async Task<ScheduledShift> ScheduleShift(ScheduledShiftUpdateDto dto)
        {
            var patrol = await _patrolRepository.GetPatrol(dto.PatrolId);

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
                var localizedStart = dto.StartsAt.Value.UtcToPatrolLocal(patrol);
                var localizedEnd = dto.EndsAt.Value.UtcToPatrolLocal(patrol);
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
                    dto.StartsAt = start.UtcFromPatrolLocal(patrol);
                }
                if(!dto.EndsAt.HasValue)
                {
                    var end = new DateTime(dto.Day.Value.Year, dto.Day.Value.Month, dto.Day.Value.Day, shift.EndHour, shift.EndMinute, 0, 0, DateTimeKind.Unspecified);
                    dto.EndsAt = end.UtcFromPatrolLocal(patrol);
                }
            }
            else
            {
                throw new InvalidOperationException("Scheduled shift must provide either start+end or day+shift");
            }

            //by this point we should have filled in the dto as best we can and can now persist

            ScheduledShift scheduledShift = null;
            List<ScheduledShiftAssignment> assignments = null;
            List<ScheduledShiftAssignment> newAssignments = new List<ScheduledShiftAssignment>();
            if(dto.Id!=default(int))
            {
                scheduledShift = await _shiftRepository.GetScheduledShift(dto.Id);

                scheduledShift.StartsAt = dto.StartsAt.Value;
                scheduledShift.EndsAt = dto.EndsAt.Value;
                scheduledShift.GroupId = group != null ? (int?)group.Id : null;
                scheduledShift.ShiftId = shift != null ? (int?)shift.Id : null;
                scheduledShift.DurationSeconds = (int)(scheduledShift.EndsAt - scheduledShift.StartsAt).TotalSeconds;
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
                        scheduledShift.DurationSeconds = (int)(scheduledShift.EndsAt - scheduledShift.StartsAt).TotalSeconds;
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
                    scheduledShift.DurationSeconds = (int)(scheduledShift.EndsAt - scheduledShift.StartsAt).TotalSeconds;
                    await _shiftRepository.InsertScheduledShift(scheduledShift);
                    assignments = new List<ScheduledShiftAssignment>();

                    //populate with work items if necassary
                    await _shiftWorkItemService.AddWorkItemsToNewShiftOccurence(scheduledShift);
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
                        newAssignments.Add(ssa);
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
                    if (existing == null || !id.HasValue)
                    {
                        var ssa = new ScheduledShiftAssignment()
                        {
                            Status = id.HasValue ? ShiftStatus.Assigned : ShiftStatus.Released,
                            AssignedUserId = id,
                            OriginalAssignedUserId = id,
                            ScheduledShiftId = scheduledShift.Id
                        };
                        assignments.Add(ssa);
                        await _shiftRepository.InsertScheduledShiftAssignment(ssa);
                        newAssignments.Add(ssa);
                    }
                    else if(existing.Status == ShiftStatus.Claimed && existing.ClaimedByUserId == id)
                    {
                        existing.AssignedUserId = existing.ClaimedByUserId.Value;
                        existing.Status = ShiftStatus.Assigned;
                        await _shiftRepository.UpdateScheduledShiftAssignment(existing);
                    }
                }
            }

            var assigneeUserIds = newAssignments.Where(x => x.AssignedUserId.HasValue).Select(x => x.AssignedUserId.Value).Distinct().ToList();
            if (assigneeUserIds.Count > 0)
            {
                var newAssignees = await _userRepository.GetUsers(assigneeUserIds);
                await _emailService.SendShiftAdded(newAssignees.ToList(), patrol, scheduledShift);
            }

            //todo: delete things that aren't there?  seems risky if we found the shift by time not id etc   

            return await _shiftRepository.GetScheduledShift(scheduledShift.Id);
        }
        public async Task CancelScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var scheduledShiftAssignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            var shift = await _shiftRepository.GetScheduledShift(scheduledShiftAssignment.ScheduledShiftId);
            var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);

            await _shiftRepository.DeleteScheduledShiftAssignment(scheduledShiftAssignment);

            //notify trainees
            var trainees = await _shiftRepository.GetTrainees(scheduledShiftAssignment.Id);
            if (trainees.Any() && scheduledShiftAssignment.AssignedUserId.HasValue)
            {
                var trainer = await _userRepository.GetUser(scheduledShiftAssignment.AssignedUserId.Value);
                var traineeUsers = await _userRepository.GetUsers(trainees.Select(x => x.TraineeUserId).ToList());
                await _emailService.SendTrainerShiftRemoved(trainer, traineeUsers.ToList(), patrol, shift);
            }

            //notify user in the shift
            if (scheduledShiftAssignment.AssignedUserId.HasValue)
            {
                var assignedUser = await _userRepository.GetUser(scheduledShiftAssignment.AssignedUserId.Value);
                await _emailService.SendShiftRemoved(assignedUser, patrol, shift);
            }
        }
        public async Task<ScheduledShiftAssignment> AddScheduledShiftAssignment(int scheduledShiftId, int? userId)
        {
            var scheduledShiftAssignment = new ScheduledShiftAssignment()
            {
                AssignedUserId = userId,
                OriginalAssignedUserId = userId,
                ScheduledShiftId = scheduledShiftId,
                Status = userId.HasValue ? ShiftStatus.Assigned : ShiftStatus.Released
            };
            await _shiftRepository.InsertScheduledShiftAssignment(scheduledShiftAssignment);

            //notify assignee
            if (userId.HasValue)
            {
                var shift = await _shiftRepository.GetScheduledShift(scheduledShiftId);
                var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);
                var newAssignee = await _userRepository.GetUser(userId.Value);
                await _emailService.SendShiftAdded(new List<User>() { newAssignee }, patrol, shift);

                //allocate any work items for the shift to the new assignee
                await _shiftWorkItemService.AddWorkItemsToNewShiftOccurence(shift);
            }

            return scheduledShiftAssignment;
        }

        public async Task<IEnumerable<ScheduledShiftAssignmentDto>> ReplicatePeriod(int patrolId, bool clearTargetPeriodFirst, bool testOnly, DateTime replicatedPeriodStart, DateTime replicatedPeriodEnd, DateTime targetPeriodStart, DateTime targetPeriodEnd, bool replicateWorkItems = true)
        {
            var patrol = await _patrolRepository.GetPatrol(patrolId);

            var createdShifts = new List<ScheduledShiftAssignmentDto>();

            if(replicatedPeriodEnd<replicatedPeriodStart)
            {
                throw new InvalidOperationException("replicated period start must be before replicated period end");
            }
            if(targetPeriodStart < replicatedPeriodEnd && targetPeriodEnd > replicatedPeriodStart)
            {
                throw new InvalidOperationException("target period cannot intersect replicated period");
            }

            DateTime replicatedStartUtc;
            DateTime replicatedEndUtc;
            DateTime targetStartUtc;
            DateTime targetEndUtc;

            if (replicatedPeriodStart.Kind != DateTimeKind.Utc)
            {
                var replicatedStartLocal = new DateTime(replicatedPeriodStart.Year, replicatedPeriodStart.Month, replicatedPeriodEnd.Day, 0, 0, 0, DateTimeKind.Unspecified);
                replicatedStartUtc = replicatedStartLocal.UtcFromPatrolLocal(patrol); // TimeZoneInfo.ConvertTimeToUtc(replicatedStartLocal, timeZone);
            }
            else
            {
                replicatedStartUtc = replicatedPeriodStart;
            }

            if (replicatedPeriodEnd.Kind != DateTimeKind.Utc)
            {
                var replicatedEndLocal = new DateTime(replicatedPeriodEnd.Year, replicatedPeriodEnd.Month, replicatedPeriodEnd.Day, 23, 59, 59, 999, DateTimeKind.Unspecified);
                replicatedEndUtc = replicatedEndLocal.UtcFromPatrolLocal(patrol); //TimeZoneInfo.ConvertTimeToUtc(replicatedEndLocal, timeZone);
            }
            else
            {
                replicatedEndUtc = replicatedPeriodEnd;
            }

            if (targetPeriodStart.Kind != DateTimeKind.Utc)
            {
                var targetStartLocal = new DateTime(targetPeriodStart.Year, targetPeriodStart.Month, targetPeriodStart.Day, 0, 0, 0, DateTimeKind.Unspecified);
                targetStartUtc = targetStartLocal.UtcFromPatrolLocal(patrol);// TimeZoneInfo.ConvertTimeToUtc(targetStartLocal, timeZone);
            }
            else
            {
                targetStartUtc = targetPeriodStart;
            }

            if (targetPeriodEnd.Kind != DateTimeKind.Utc)
            {
                var targetEndLocal = new DateTime(targetPeriodEnd.Year, targetPeriodEnd.Month, targetPeriodEnd.Day, 23, 59, 59, 999, DateTimeKind.Unspecified);
                targetEndUtc = targetEndLocal.UtcFromPatrolLocal(patrol);//TimeZoneInfo.ConvertTimeToUtc(targetEndLocal, timeZone);
            }
            else
            {
                targetEndUtc = targetPeriodEnd;
            }

            if (clearTargetPeriodFirst && !testOnly)
            {
                var shiftsToRemove = await _shiftRepository.GetScheduledShiftAssignments(patrolId, null, targetStartUtc, targetEndUtc, null);
                var scheduledShiftIds = shiftsToRemove.Select(x => x.ScheduledShiftId).Distinct().ToList();
                foreach(var id in scheduledShiftIds)
                {
                    await this.CancelShift(id);
                }
            }

            var shiftsToCopy = await _shiftRepository.GetScheduledShiftAssignments(patrolId, null, replicatedStartUtc, replicatedEndUtc, null);
            //foreach (var shift in shiftsToCopy)
            //{
            //    shift.StartsAt = shift.StartsAt.UtcToPatrolLocal(patrol);
            //    shift.EndsAt = shift.EndsAt.UtcToPatrolLocal(patrol);
            //}

            //group and index by day
            var grouped = shiftsToCopy.GroupBy(x => new { x.StartsAt.Year, x.StartsAt.Month, x.StartsAt.Day })
                .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month).ThenBy(x => x.Key.Day)
                .ToList();

            //step a day at a time through the target range
            for(int i=0;targetStartUtc + new TimeSpan(i,0,0,0) < targetEndUtc;i++)
            {
                var day = targetStartUtc + new TimeSpan(i, 0, 0, 0);
                var group = grouped[i % grouped.Count].GroupBy(x=>x.ScheduledShiftId);
                
                foreach(var scheduledShift in group)
                {
                    if (!testOnly)
                    {
                        var createdShift = await this.ScheduleShift(new ScheduledShiftUpdateDto()
                        {
                            GroupId = scheduledShift.First().Group?.Id,
                            ShiftId = scheduledShift.First().Shift?.Id,
                            PatrolId = patrolId,
                            Day = scheduledShift.First().Shift != null ? day : (DateTime?)null,
                            StartsAt = scheduledShift.First().Shift == null ? (new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().StartsAt.Hour, scheduledShift.First().StartsAt.Minute, 0, DateTimeKind.Utc)) : (DateTime?)null,
                            EndsAt = scheduledShift.First().Shift == null ? (new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().EndsAt.Hour, scheduledShift.First().EndsAt.Minute, 0, DateTimeKind.Utc)) : (DateTime?)null,
                            AssignUserIds = scheduledShift.Select(x => x.AssignedUser != null ? x.AssignedUser.Id : (int?)null).Distinct().ToList()
                        });
                        createdShifts.Add(new ScheduledShiftAssignmentDto()
                        {
                            ScheduledShiftId = createdShift.Id,
                            StartsAt = createdShift.StartsAt,
                            EndsAt = createdShift.EndsAt,
                            Group = scheduledShift.First().Group,
                            Shift = scheduledShift.First().Shift
                        });

                        if (replicateWorkItems)
                        {
                            await _shiftWorkItemService.AddWorkItemsToNewShiftOccurence(createdShift);
                        }
                    }
                    else
                    {
                        var resultStart = new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().StartsAt.Hour, scheduledShift.First().StartsAt.Minute, 0, DateTimeKind.Utc);
                        var resultEnd = new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().EndsAt.Hour, scheduledShift.First().EndsAt.Minute, 0, DateTimeKind.Utc);

                        //if(scheduledShift.First().Shift!=null)
                        //{
                            //resultStart = new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().Shift.StartHour, scheduledShift.First().Shift.StartMinute, 0, DateTimeKind.Unspecified).UtcFromPatrolLocal(patrol);
                            //resultEnd = new DateTime(day.Year, day.Month, day.Day, scheduledShift.First().Shift.EndHour, scheduledShift.First().Shift.EndMinute, 0, DateTimeKind.Unspecified).UtcFromPatrolLocal(patrol);
                        //}

                        createdShifts.Add(new ScheduledShiftAssignmentDto()
                        {
                            Group = scheduledShift.First().Group,
                            Shift = scheduledShift.First().Shift,
                            StartsAt = resultStart,
                            EndsAt = resultEnd,
                        });
                    }
                }
            }

            return createdShifts;
        }
    }
}
