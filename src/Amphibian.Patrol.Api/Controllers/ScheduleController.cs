using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Infrastructure;
using Amphibian.Patrol.Api.Services;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly IShiftRepository _shiftRepository;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IScheduleService _scheduleService;
        private readonly ISystemClock _clock;

        public ScheduleController(ILogger<ScheduleController> logger,IScheduleService scheduleService, IShiftRepository shiftRepository, IPatrolRepository patrolRepository, ISystemClock clock)
        {
            _logger = logger;
            _shiftRepository = shiftRepository;
            _patrolRepository = patrolRepository;
            _clock = clock;
            _scheduleService = scheduleService;
        }

        public class ScheduleQuery
        {
            public int PatrolId { get; set; }
            public int? UserId { get; set; }
            public ShiftStatus? Status { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
        [HttpPost]
        [Route("schedule/search")]
        [Authorize]
        public async Task<IActionResult> GetSchedule(ScheduleQuery query)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            if (patrols.Any(x => x.Id == query.PatrolId))
            {
                var shiftAssignments = await _shiftRepository.GetScheduledShiftAssignments(query.PatrolId, query.UserId, query.From, query.To, query.Status);

                var grouped = shiftAssignments.GroupBy(x => new { x.ScheduledShiftId,
                    GroupId = x.Group?.Id,
                    GroupName = x.Group?.Name,
                    ShiftId = x.Shift?.Id,
                    ShiftName = x.Shift?.Name, x.StartsAt, x.EndsAt }).Select(x => new
                {
                    x.Key.ScheduledShiftId,
                    GroupId = x.Key.GroupId,
                    GroupName = x.Key.GroupName,
                    ShiftId = x.Key.ShiftId,
                    ShiftName = x.Key.ShiftName,
                    x.Key.StartsAt,
                    x.Key.EndsAt,
                    Assignments = x.Select(y => new
                    {
                        y.Id,
                        y.AssignedUser,
                        y.ClaimedByUser,
                        y.OriginalAssignedUser,
                        y.Status,
                        y.TraineeCount,
                    })
                });

                return Ok(grouped);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
