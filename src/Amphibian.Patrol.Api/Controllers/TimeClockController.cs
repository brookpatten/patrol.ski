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
        
        public TimeClockController(ILogger<AnnouncementController> logger, IPatrolRepository patrolRepository,
            IPatrolService patrolService,ITimeClockService timeClockService, ITimeEntryRepository timeEntryRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolService = patrolService;
            _timeClockService = timeClockService;
            _timeEntryRepository = timeEntryRepository;
        }

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

                var entry = await _timeClockService.ClockIn(patrolId, userId.Value);
                return Ok(entry);
            }
            else
            {
                return Forbid();
            }
        }

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
                entry = await _timeClockService.ClockOut(timeEntryid);
                return Ok(entry);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
