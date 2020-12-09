using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class TimeClockController : ControllerBase
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private IPatrolService _patrolService;
        private ITimeClockService _timeClockService;
        private ITimeEntryRepository _timeEntryRepository;
        private ISystemClock _clock;
        
        public TimeClockController(ILogger<AnnouncementController> logger, IPatrolRepository patrolRepository,
            IPatrolService patrolService,ITimeClockService timeClockService, ITimeEntryRepository timeEntryRepository, ISystemClock clock)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolService = patrolService;
            _timeClockService = timeClockService;
            _timeEntryRepository = timeEntryRepository;
            _clock = clock;
        }

        /// <summary>
        /// clock in the current user into the specified patrol
        /// </summary>
        /// <param name="patrolId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("timeclock/clock-in/{patrolId}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ClockIn(int patrolId, int? userId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            if (patrols.Any(x => x.Id == patrolId) 
                || (await _patrolService.GetUserRoleInPatrol(User.GetUserId(),patrolId)).CanMaintainTimeClock()) //or user is an admin
            {
                if(!userId.HasValue)
                {
                    userId = User.GetUserId();
                }

                var result = await _timeClockService.ClockIn(patrolId, userId.Value);
                return Ok(result);
            }
            else
            {
                return Forbid();
            }
        }

        /// <summary>
        /// clock out the specified user/timeentry
        /// </summary>
        /// <param name="timeEntryid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("timeclock/clock-out/{timeEntryid}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ClockOut(int timeEntryid)
        {
            var entry = await _timeEntryRepository.GetTimeEntry(timeEntryid);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == entry.PatrolId) && 
                (entry.UserId==User.GetUserId() 
                || (await _patrolService.GetUserRoleInPatrol(User.GetUserId(),entry.PatrolId)).CanMaintainTimeClock())) //or the user is an admin
            {
                var result = await _timeClockService.ClockOut(timeEntryid);
                return Ok(result);
            }
            else
            {
                return Forbid();
            }
        }

        public class TimeclockEntryQueryDto
        {
            public int PatrolId { get; set; }
            public int? UserId { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
        /// <summary>
        /// search for time clock entries
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("timeclock/entries")]
        [Authorize]
        public async Task<IActionResult> GetEntries(TimeclockEntryQueryDto dto)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == dto.PatrolId) &&
                (dto.UserId == User.GetUserId()
                || (await _patrolService.GetUserRoleInPatrol(User.GetUserId(), dto.PatrolId)).CanMaintainTimeClock())) //or the user is an admin
            {
                var entries = await _timeEntryRepository.GetTimeEntries(dto.PatrolId, dto.UserId, dto.From, dto.To);
                return Ok(entries);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("timeclock/days")]
        [Authorize]
        public async Task<IActionResult> GetDays(TimeclockEntryQueryDto dto)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == dto.PatrolId) &&
                (dto.UserId == User.GetUserId()
                || (await _patrolService.GetUserRoleInPatrol(User.GetUserId(), dto.PatrolId)).CanMaintainTimeClock())) //or the user is an admin
            {
                var entries = await _timeEntryRepository.GetTimeEntries(dto.PatrolId, dto.UserId, dto.From, dto.To);

                var groups = entries
                    .GroupBy(x => new { x.TimeEntry.ClockIn.Year, x.TimeEntry.ClockIn.Month, x.TimeEntry.ClockIn.Day, x.User.Id })
                    .Select(x => 
                    new CurrentTimeEntryDto()
                    {
                        TimeEntry = new TimeEntry()
                        {
                            ClockIn = new DateTime(x.Key.Year,x.Key.Month,x.Key.Day,0,0,0,DateTimeKind.Utc),
                            //actual worked
                            DurationSeconds = x.GroupBy(y=>y.TimeEntry.Id).Select(y=>y.First()).Sum(y=>y.TimeEntry.DurationSeconds)
                        },
                        TimeEntryScheduledShiftAssignment = new TimeEntryScheduledShiftAssignment()
                        {
                            //worked in shift
                            DurationSeconds = x.Where(y=>y.TimeEntryScheduledShiftAssignment!=null).GroupBy(y=>y.TimeEntryScheduledShiftAssignment.Id).Select(y=>y.First()).Sum(y=>y.TimeEntryScheduledShiftAssignment.DurationSeconds)
                        },
                        ScheduledShift = new ScheduledShift()
                        {
                            //scheduled duration in the day
                            DurationSeconds = x.Where(y=>y.ScheduledShift!=null).GroupBy(y=>y.ScheduledShift.Id).Select(y=>y.First()).Sum(y=>y.ScheduledShift.DurationSeconds)
                        },
                        User = x.First().User
                    });;

                return Ok(groups);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("timeclock/missing")]
        [Authorize]
        public async Task<IActionResult> GetMissing(TimeclockEntryQueryDto dto)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == dto.PatrolId) &&
                (dto.UserId == User.GetUserId()
                || (await _patrolService.GetUserRoleInPatrol(User.GetUserId(), dto.PatrolId)).CanMaintainTimeClock())) //or the user is an admin
            {
                var entries = await _timeEntryRepository.GetMissingShiftTime(dto.PatrolId, dto.UserId, dto.From, dto.To);

                return Ok(entries);
            }
            else
            {
                return Forbid();
            }
        }

        /// <summary>
        /// returns currently clocked in people
        /// </summary>
        /// <param name="patrolId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("timeclock/active/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetActive(int patrolId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == patrolId)) //or the user is an admin
            {
                var activeEntries = await _timeEntryRepository.GetActiveTimeEntries(patrolId, _clock.UtcNow.UtcDateTime);
                return Ok(activeEntries);
            }
            else
            {
                return Forbid();
            }
        }

        /// <summary>
        /// returns all late or missing people from currently active shifts
        /// </summary>
        /// <param name="patrolId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("timeclock/late/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetLate(int patrolId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == patrolId)) //or the user is an admin
            {
                var activeEntries = await _timeEntryRepository.GetMissingTimeEntries(patrolId, _clock.UtcNow.UtcDateTime);
                return Ok(activeEntries);
            }
            else
            {
                return Forbid();
            }
        }

        /// <summary>
        /// returns the current active time entry for the authenticated user in the specified patrol
        /// </summary>
        /// <param name="patrolId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("timeclock/current/mine/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetCurrent(int patrolId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            //ensure the entry belongs to the current user and that the current user is still in the specified patrol
            if (patrols.Any(x => x.Id == patrolId)) //or the user is an admin
            {
                var result = await _timeClockService.GetCurrent(patrolId, User.GetUserId());
                return Ok(result);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
