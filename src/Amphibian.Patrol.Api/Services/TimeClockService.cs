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

        public TimeClockService(ILogger<TimeClockService> logger,ITimeEntryRepository timeEntryRepository,
            IPatrolRepository patrolRepository,IShiftRepository shiftRepository, ISystemClock systemClock)
        {
            _logger = logger;
            _timeEntryRepository = timeEntryRepository;
            _patrolRepository = patrolRepository;
            _shiftRepository = shiftRepository;
            _systemClock = systemClock;
        }

        public async Task<TimeEntry> ClockIn(int patrolId, int userId)
        {
            var existingEntries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, userId);
            //if there are existing entries we don't need to do anything
            if(!existingEntries.Any())
            {
                var entry = new TimeEntry()
                {
                    ClockIn = _systemClock.UtcNow.DateTime,
                    PatrolId = patrolId,
                    UserId = userId
                };
                await _timeEntryRepository.InsertTimeEntry(entry);

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
                        var timeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
                        {
                            ScheduledShiftAssignmentId = shift.Id,
                            TimeEntryId = entry.Id
                        };
                        await _timeEntryRepository.InsertTimeEntryScheduledShiftAssignment(timeEntryScheduledShiftAssignment);
                    }
                }

                return entry;
            }
            else
            {
                return existingEntries.OrderBy(x => x.ClockIn).First();
            }
        }

        public async Task<TimeEntry> ClockOut(int timeEntryId)
        {
            var entry = await _timeEntryRepository.GetTimeEntry(timeEntryId);

            //make sure it's not already been clocked out
            if(!entry.ClockOut.HasValue)
            {
                entry.ClockOut = _systemClock.UtcNow.DateTime;
                entry.DurationSeconds = (int)(entry.ClockOut.Value - entry.ClockIn).TotalSeconds;
                await _timeEntryRepository.UpdateTimeEntry(entry);

                //if scheduling is enabled, complete entry=>schedule relating/calculating
                var patrol = await _patrolRepository.GetPatrol(entry.PatrolId);
                if(patrol.EnableScheduling)
                {
                    //track all seconds that need to be allocated from the current time entry
                    int unallocatedSeconds = entry.DurationSeconds.Value;
                    int allocatedSeconds = 0;

                    var shifts = (await _shiftRepository.GetScheduledShiftAssignments(entry.PatrolId, entry.UserId, entry.ClockIn, entry.ClockOut, ShiftStatus.Assigned)).ToList();
                    shifts = shifts.OrderBy(x => x.StartsAt).ToList();

                    var timeEntryScheduledShiftAssignments = new List<TimeEntryScheduledShiftAssignment>();
                    timeEntryScheduledShiftAssignments.AddRange(await _timeEntryRepository.GetScheduledShiftAssignmentsForTimeEntry(entry.Id));

                    for(int i=0;i<shifts.Count && unallocatedSeconds > 0;i++)
                    {
                        var shift = shifts[i];

                        //time entries related to this shift
                        var otherEntryAllocatedScheduledShiftAssignments = (await _timeEntryRepository.GetScheduledShiftAssignmentsForScheduledShiftAssignment(shift.Id)).ToList();
                        otherEntryAllocatedScheduledShiftAssignments = otherEntryAllocatedScheduledShiftAssignments.Where(x => x.ScheduledShiftAssignmentId != shift.Id).ToList();
                        
                        //get the schedule entry so we can see how much can be allocated
                        var scheduledShift = await _shiftRepository.GetScheduledShift(shift.ScheduledShiftId);

                        //see if this schedule entry is already covered by previous time entries
                        if(otherEntryAllocatedScheduledShiftAssignments.Sum(x=>x.DurationSeconds) < scheduledShift.DurationSeconds)
                        {
                            //if not, allocate what we can to this shift
                            //offsetting by allocatedSeconds prevents overlapping shifts from both being allocated to the same time
                            var allocateStart = entry.ClockIn + new TimeSpan(0,0,allocatedSeconds) > scheduledShift.StartsAt ? entry.ClockIn + new TimeSpan(0, 0, allocatedSeconds) : scheduledShift.StartsAt;
                            var allocateEnd = entry.ClockOut.Value > scheduledShift.EndsAt ? scheduledShift.EndsAt : entry.ClockOut.Value;

                            var maximumOverlappedAvailableAllocationSeconds = (int)(allocateEnd - allocateStart).TotalSeconds;
                            var neededSeconds = scheduledShift.DurationSeconds - otherEntryAllocatedScheduledShiftAssignments.Sum(x => x.DurationSeconds);

                            //figure out how much to allocate based on overlap and what's remaining
                            var secondsToAllocate = maximumOverlappedAvailableAllocationSeconds > neededSeconds ? neededSeconds : maximumOverlappedAvailableAllocationSeconds;
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

            return entry;
        }
    }
}
