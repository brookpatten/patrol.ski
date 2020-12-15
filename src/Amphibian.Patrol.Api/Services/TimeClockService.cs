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
    public class TimeClockService : ITimeClockService
    {
        private ILogger<TimeClockService> _logger;
        private ITimeEntryRepository _timeEntryRepository;
        private IPatrolRepository _patrolRepository;
        private IShiftRepository _shiftRepository;
        private ISystemClock _systemClock;
        private IGroupRepository _groupRepository;

        public TimeClockService(ILogger<TimeClockService> logger,ITimeEntryRepository timeEntryRepository,
            IPatrolRepository patrolRepository,IShiftRepository shiftRepository, ISystemClock systemClock, IGroupRepository groupRepository)
        {
            _logger = logger;
            _timeEntryRepository = timeEntryRepository;
            _patrolRepository = patrolRepository;
            _shiftRepository = shiftRepository;
            _systemClock = systemClock;
            _groupRepository = groupRepository;
        }

        public async Task<CurrentTimeEntryDto> ClockIn(int patrolId, int userId, DateTime? now=null)
        {
            if(!now.HasValue)
            {
                now = _systemClock.UtcNow.UtcDateTime;
            }
            var existingEntries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, userId);
            //if there are existing entries we don't need to do anything
            if(!existingEntries.Any())
            {
                var result = new CurrentTimeEntryDto();
                var entry = new TimeEntry()
                {
                    ClockIn = now.Value,
                    PatrolId = patrolId,
                    UserId = userId
                };
                await _timeEntryRepository.InsertTimeEntry(entry);
                result.TimeEntry = entry;

                //if the patrol has schedulingenabled, try to find a shift to associate
                var patrol = await _patrolRepository.GetPatrol(patrolId);
                if(patrol.EnableScheduling)
                {
                    //find shifts for the user that occur in the next day
                    //TODO: does this range need to be configurable by patrol?  eg: how early can you allow clocking in?
                    var shifts = await _shiftRepository.GetScheduledShiftAssignments(patrolId, userId, entry.ClockIn, entry.ClockIn + new TimeSpan(1,0,0,0), ShiftStatus.Assigned);
                    shifts = shifts.OrderBy(x => x.StartsAt);
                    if(shifts.Any())
                    {
                        //if there is more than one match, associate the one that starts the earliest
                        var shift = shifts.First();
                        var scheduledShift = await _shiftRepository.GetScheduledShift(shift.ScheduledShiftId);
                        var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
                        {
                            ScheduledShiftAssignmentId = shift.Id,
                            TimeEntryId = entry.Id
                        };
                        await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);
                        
                        result.TimeEntryScheduledShiftAssignment = timeEntryScheduledShiftAssignment;
                        result.ScheduledShift = scheduledShift;
                        if(scheduledShift.ShiftId.HasValue)
                        {
                            result.Shift = await _shiftRepository.GetShift(scheduledShift.ShiftId.Value);
                        }
                        if(scheduledShift.GroupId.HasValue)
                        {
                            result.Group = await _groupRepository.GetGroup(scheduledShift.GroupId.Value);
                        }
                    }
                }

                return result;
            }
            else
            {
                var result = new CurrentTimeEntryDto();
                var entry = existingEntries.Where(x => x.ClockIn < now).OrderBy(x => x.ClockIn).FirstOrDefault();

                var timeEntryScheduledShiftAssignments = await _timeEntryRepository.GetScheduledShiftAssignmentsForTimeEntry(entry.Id);
                if (timeEntryScheduledShiftAssignments.Any())
                {
                    var tessa = timeEntryScheduledShiftAssignments.FirstOrDefault();
                    result.TimeEntryScheduledShiftAssignment = tessa;
                    var assignment = await _shiftRepository.GetScheduledShiftAssignment(tessa.ScheduledShiftAssignmentId);
                    result.ScheduledShift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
                    if (result.ScheduledShift.ShiftId.HasValue)
                    {
                        result.Shift = await _shiftRepository.GetShift(result.ScheduledShift.ShiftId.Value);
                    }
                    if (result.ScheduledShift.GroupId.HasValue)
                    {
                        result.Group = await _groupRepository.GetGroup(result.ScheduledShift.GroupId.Value);
                    }
                }

                return result;
            }
        }

        public async Task<CurrentTimeEntryDto> GetCurrent(int patrolId, int userId)
        {
            var result = new CurrentTimeEntryDto();
            var activeEntries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, userId);
            result.TimeEntry = activeEntries.OrderBy(x => x.ClockIn).FirstOrDefault();
            if(result.TimeEntry!=null)
            {
                var tessas = await _timeEntryRepository.GetScheduledShiftAssignmentsForTimeEntry(result.TimeEntry.Id);
                result.TimeEntryScheduledShiftAssignment = tessas.OrderBy(x => x.Id).FirstOrDefault();

                if(result.TimeEntryScheduledShiftAssignment!=null)
                {
                    var scheduledShiftAssignment = await _shiftRepository.GetScheduledShiftAssignment(result.TimeEntryScheduledShiftAssignment.ScheduledShiftAssignmentId);
                    result.ScheduledShift = await _shiftRepository.GetScheduledShift(scheduledShiftAssignment.ScheduledShiftId);

                    if (result.ScheduledShift.ShiftId.HasValue)
                    {
                        result.Shift = await _shiftRepository.GetShift(result.ScheduledShift.ShiftId.Value);
                    }
                    if (result.ScheduledShift.GroupId.HasValue)
                    {
                        result.Group = await _groupRepository.GetGroup(result.ScheduledShift.GroupId.Value);
                    }
                }
            }
            else
            {
                //find the first shift in the next week
                var scheduledShifts = await _shiftRepository.GetScheduledShiftAssignments(patrolId, userId, _systemClock.UtcNow.UtcDateTime, _systemClock.UtcNow.UtcDateTime + new TimeSpan(7,0, 0, 0));
                var scheduledShiftDto = scheduledShifts.OrderBy(x => x.StartsAt).FirstOrDefault();
                if(scheduledShiftDto != null)
                {
                    result.ScheduledShift = await _shiftRepository.GetScheduledShift(scheduledShiftDto.ScheduledShiftId);
                    if (result.ScheduledShift.ShiftId.HasValue)
                    {
                        result.Shift = await _shiftRepository.GetShift(result.ScheduledShift.ShiftId.Value);
                    }
                    if (result.ScheduledShift.GroupId.HasValue)
                    {
                        result.Group = await _groupRepository.GetGroup(result.ScheduledShift.GroupId.Value);
                    }
                }
            }
            return result;
        }

        public async Task<CurrentTimeEntryDto> ClockOut(int timeEntryId, DateTime? now=null)
        {
            if (!now.HasValue)
            {
                now = _systemClock.UtcNow.UtcDateTime;
            }

            var result = new CurrentTimeEntryDto();
            var entry = await _timeEntryRepository.GetTimeEntry(timeEntryId);
            result.TimeEntry = entry;

            //make sure it's not already been clocked out
            if(!entry.ClockOut.HasValue)
            {
                entry.ClockOut = now.Value;
                entry.DurationSeconds = (int)(entry.ClockOut.Value - entry.ClockIn).TotalSeconds;
                await _timeEntryRepository.UpdateTimeEntry(entry);

                //if scheduling is enabled, complete entry=>schedule relating/calculating
                var patrol = await _patrolRepository.GetPatrol(entry.PatrolId);
                if(patrol.EnableScheduling)
                {
                    //track all seconds that need to be allocated from the current time entry
                    int unallocatedSeconds = entry.DurationSeconds.Value;
                    //track what has been allocated thus far
                    int allocatedSeconds = 0;

                    var shifts = (await _shiftRepository.GetScheduledShiftAssignments(entry.PatrolId, entry.UserId, entry.ClockIn, entry.ClockOut)).ToList();
                    shifts = shifts.OrderBy(x => x.StartsAt).ToList();

                    var timeEntryScheduledShiftAssignments = new List<TimeEntryScheduledShiftAssignment>();
                    timeEntryScheduledShiftAssignments.AddRange(await _timeEntryRepository.GetScheduledShiftAssignmentsForTimeEntry(entry.Id));
                    //reset all existing allocations to 0
                    foreach(var existing in timeEntryScheduledShiftAssignments)
                    {
                        existing.DurationSeconds = 0;
                    }

                    for(int i=0;i<shifts.Count && unallocatedSeconds > 0;i++)
                    {
                        var shift = shifts[i];

                        //time entries related to this shift
                        var otherEntryAllocatedScheduledShiftAssignments = (await _timeEntryRepository.GetScheduledShiftAssignmentsForScheduledShiftAssignment(shift.Id)).ToList();
                        otherEntryAllocatedScheduledShiftAssignments = otherEntryAllocatedScheduledShiftAssignments.Where(x => x.TimeEntryId != entry.Id).ToList();
                        
                        //get the schedule entry so we can see how much can be allocated
                        var scheduledShift = await _shiftRepository.GetScheduledShift(shift.ScheduledShiftId);
                        result.ScheduledShift = scheduledShift;

                        //see if this schedule entry is already covered by previous time entries
                        int previouslyAllocatedShiftSeconds = otherEntryAllocatedScheduledShiftAssignments.Sum(x => x.DurationSeconds);
                        if (previouslyAllocatedShiftSeconds < scheduledShift.DurationSeconds)
                        {
                            //if not, allocate what we can to this shift
                            //offsetting by allocatedSeconds and previouslyAllocatedShiftSeconds prevents overlapping shifts from both being allocated to the same time
                            var allocateStart = entry.ClockIn + new TimeSpan(0,0,allocatedSeconds) > scheduledShift.StartsAt + new TimeSpan(0,0, previouslyAllocatedShiftSeconds) ? entry.ClockIn + new TimeSpan(0, 0, allocatedSeconds) : scheduledShift.StartsAt + new TimeSpan(0, 0, previouslyAllocatedShiftSeconds);
                            var allocateEnd = entry.ClockOut.Value > scheduledShift.EndsAt ? scheduledShift.EndsAt : entry.ClockOut.Value;

                            //TODO: we could check for existing time entries that overlap between allocateStart/End, but that really shouldn't happen
                            //since a user can't have multiple ongoing time entries

                            var maximumOverlappedAvailableAllocationSeconds = (int)(allocateEnd - allocateStart).TotalSeconds;
                            var neededSeconds = scheduledShift.DurationSeconds - previouslyAllocatedShiftSeconds;

                            //figure out how much to allocate based on overlap and what's remaining in the shift
                            var secondsToAllocate = maximumOverlappedAvailableAllocationSeconds > neededSeconds ? neededSeconds : maximumOverlappedAvailableAllocationSeconds;
                            //figure out how much to allocate based on what's remaining in the time entry
                            secondsToAllocate = secondsToAllocate > unallocatedSeconds ? unallocatedSeconds : secondsToAllocate;

                            //do the allocation
                            //find a entryscheduledshiftassignment to alloate with, or make one
                            var timeEntryScheduledShiftAssignment = timeEntryScheduledShiftAssignments.SingleOrDefault(x => x.ScheduledShiftAssignmentId == shift.Id);
                            if(timeEntryScheduledShiftAssignment==null)
                            {
                                timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
                                {
                                    ScheduledShiftAssignmentId = shift.Id,
                                    TimeEntryId = entry.Id,
                                    DurationSeconds = secondsToAllocate,
                                };
                                await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);
                                otherEntryAllocatedScheduledShiftAssignments.Add(timeEntryScheduledShiftAssignment);
                            }
                            else
                            {
                                timeEntryScheduledShiftAssignment.DurationSeconds = secondsToAllocate;
                                await _timeEntryRepository.UpdateTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);
                            }

                            //adjust running totals
                            unallocatedSeconds = unallocatedSeconds - secondsToAllocate;
                            allocatedSeconds = allocatedSeconds + secondsToAllocate;

                            result.TimeEntryScheduledShiftAssignment = timeEntryScheduledShiftAssignment;
                        }
                        else
                        {
                            //_logger.LogDebug("Previously allocated > current duration");
                        }

                        if (result.ScheduledShift.ShiftId.HasValue)
                        {
                            result.Shift = await _shiftRepository.GetShift(result.ScheduledShift.ShiftId.Value);
                        }
                        if (result.ScheduledShift.GroupId.HasValue)
                        {
                            result.Group = await _groupRepository.GetGroup(result.ScheduledShift.GroupId.Value);
                        }
                    }
                    
                    //there's no scenario where this should happen, the only way it could occur is if a schedule change
                    //occurred mid-shift, which we do not allow.  But just in case, clean it up anyway
                    foreach(var timeEntryScheduledShiftAssignment in timeEntryScheduledShiftAssignments)
                    {
                        if(timeEntryScheduledShiftAssignment.DurationSeconds==0)
                        {
                            await _timeEntryRepository.DeleteTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);
                        }
                    }
                }

            }

            return result;
        }
    }
}
