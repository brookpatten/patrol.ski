﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Infrastructure;
using Amphibian.Patrol.Api.Services;
using Amphibian.Patrol.Api.Dtos;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly IShiftRepository _shiftRepository;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPatrolService _patrolService;
        private readonly IScheduleService _scheduleService;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _clock;

        public ScheduleController(ILogger<ScheduleController> logger,IScheduleService scheduleService, IShiftRepository shiftRepository
            , IPatrolRepository patrolRepository, ISystemClock clock, IPatrolService patrolService, IUserRepository userRepository)
        {
            _logger = logger;
            _shiftRepository = shiftRepository;
            _patrolRepository = patrolRepository;
            _clock = clock;
            _scheduleService = scheduleService;
            _patrolService = patrolService;
            _userRepository = userRepository;
        }

        public class ScheduleQuery
        {
            public int PatrolId { get; set; }
            public int? UserId { get; set; }
            public ShiftStatus? Status { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public int? NoOverlapWithUserId { get; set; }
        }
        [HttpPost]
        [Route("schedule/search")]
        [Authorize]
        public async Task<IActionResult> GetSchedule(ScheduleQuery query)
        {
            if (User.PatrolIds().Any(x=>x==query.PatrolId))
            {
                var shiftAssignments = await _shiftRepository.GetScheduledShiftAssignments(query.PatrolId, query.UserId, query.From, query.To, query.Status, null ,query.NoOverlapWithUserId);
                return MapScheduledShiftAssessments(shiftAssignments);
            }
            else
            {
                return Forbid();
            }
        }

        private IActionResult MapScheduledShiftAssessments(IEnumerable<ScheduledShiftAssignmentDto> shiftAssignments)
        {
            var grouped = shiftAssignments.GroupBy(x => new {
                x.ScheduledShiftId,
                GroupId = x.Group?.Id,
                GroupName = x.Group?.Name,
                ShiftId = x.Shift?.Id,
                ShiftName = x.Shift?.Name,
                x.StartsAt,
                x.EndsAt
            }).Select(x => new
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
                    x.Key.ScheduledShiftId
                })
            });

            return Ok(grouped);
        }

        [HttpPost]
        [Route("schedule/scheduled-shift-assignment")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> AddScheduledShiftAssignment(int scheduledShiftId,int? userId)
        {
            var now = _clock.UtcNow.UtcDateTime;
            //ensure the shift exists and that the user is allowed to modify it
            var shift = await _shiftRepository.GetScheduledShift(scheduledShiftId);
            if(User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                && now < shift.StartsAt)
            {
                //ensure if the user is specified they are in the patrol
                PatrolUser patrolUser = null;
                if(userId.HasValue)
                {
                    patrolUser = await _patrolRepository.GetPatrolUser(userId.Value, shift.PatrolId);
                }
                if(!userId.HasValue || patrolUser!=null)
                {
                    var assignment = await _scheduleService.AddScheduledShiftAssignment(scheduledShiftId, userId);

                    var scheduledShift = await _shiftRepository.GetScheduledShiftAssignments(shift.PatrolId, scheduledShiftId: scheduledShiftId);
                    return MapScheduledShiftAssessments(scheduledShift);
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete]
        [Route("schedule/scheduled-shift-assignment")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CancelScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var assignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            //ensure the shift exists and that the user is allowed to modify it
            var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
            if (User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                && now < shift.StartsAt)
            {
                await _scheduleService.CancelScheduledShiftAssignment(scheduledShiftAssignmentId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/scheduled-shift-assignment/release")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ReleaseScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var assignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
            if ((//is an admin
                User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                //shift belongs to current user
                || assignment.AssignedUserId == User.UserId())
                && now < shift.StartsAt)
            {
                await _scheduleService.ReleaseShift(scheduledShiftAssignmentId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/scheduled-shift-assignment/cancel-release")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CancelReleaseScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var assignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
            if ((//is an admin
                User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                //shift belongs to current user
                || assignment.AssignedUserId == User.UserId())
                && now < shift.StartsAt)
            {
                await _scheduleService.CancelShiftRelease(scheduledShiftAssignmentId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/scheduled-shift-assignment/claim")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ClaimScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var assignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
            
            //if the user is in the patrol and the shift is in the future
            if (User.PatrolIds().Any(x=>x==shift.PatrolId)
                && now < shift.StartsAt)
            {
                await _scheduleService.ClaimShift(scheduledShiftAssignmentId,User.UserId());

                //HACK: get the dto but no repo method exists to get a ssadto by id...
                var assignmentDtos = await _shiftRepository.GetScheduledShiftAssignments(shift.PatrolId, scheduledShiftId: shift.Id);
                var assignmentDto = assignmentDtos.Single(x => x.Id == assignment.Id);

                return Ok(assignmentDto);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/scheduled-shift-assignment/approve")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ApproveScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var assignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            //ensure the shift exists and that the user is allowed to modify it
            var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
            if (User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                && now < shift.StartsAt)
            {
                await _scheduleService.ApproveShiftSwap(scheduledShiftAssignmentId,User.UserId());
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/scheduled-shift-assignment/reject")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> RejectScheduledShiftAssignment(int scheduledShiftAssignmentId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var assignment = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            //ensure the shift exists and that the user is allowed to modify it
            var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);
            if (User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                && now < shift.StartsAt)
            {
                await _scheduleService.RejectShiftSwap(scheduledShiftAssignmentId, User.UserId());
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete]
        [Route("schedule/scheduled-shift")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CancelScheduledShift(int scheduledShiftId)
        {
            var now = _clock.UtcNow.UtcDateTime;

            var shift = await _shiftRepository.GetScheduledShift(scheduledShiftId);
            if (User.RoleInPatrol(shift.PatrolId).CanMaintainSchedule()
                && now < shift.StartsAt)
            {
                await _scheduleService.CancelShift(scheduledShiftId);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/scheduled-shift")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateScheduledShift(ScheduledShiftUpdateDto dto)
        {
            var now = _clock.UtcNow.UtcDateTime;

            if (User.RoleInPatrol(dto.PatrolId).CanMaintainSchedule()
                &&(
                    (dto.Day.HasValue && dto.Day.Value > now)
                    || (dto.StartsAt.HasValue && dto.StartsAt.Value > now)
                ))
            {
                var shift = await _scheduleService.ScheduleShift(dto);
                return Ok(shift);
            }
            else
            {
                return Forbid();
            }
        }

        public class ReplicatePeriodDto
        {
            public int PatrolId { get; set; }
            public bool ClearTarget { get; set; }
            public DateTime SourceStart { get; set; }
            public DateTime SourceEnd { get; set; }
            public DateTime TargetStart { get; set; }
            public DateTime TargetEnd { get; set; }
            public bool TestOnly { get; set; }
        }

        [HttpPost]
        [Route("schedule/replicate")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ReplicatePeriod (ReplicatePeriodDto dto)
        {
            var now = _clock.UtcNow.UtcDateTime;

            if (User.RoleInPatrol( dto.PatrolId).CanMaintainSchedule()
                && dto.TargetStart > now)
            {
                var results = await _scheduleService.ReplicatePeriod(dto.PatrolId, dto.ClearTarget,dto.TestOnly, dto.SourceStart, dto.SourceEnd, dto.TargetStart, dto.TargetEnd);
                return Ok(results);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("schedule/shifts")]
        [Authorize]
        public async Task<IActionResult> GetShifts(int patrolid)
        {
            if (User.RoleInPatrol(patrolid).CanMaintainSchedule())
            {
                var results = await _shiftRepository.GetShifts(patrolid);
                return Ok(results);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("schedule/shift")]
        [Authorize]
        public async Task<IActionResult> GetShift(int shiftId)
        {
            var shift = await _shiftRepository.GetShift(shiftId);
            if (User.RoleInPatrol( shift.PatrolId).CanMaintainSchedule())
            {
                return Ok(shift);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("schedule/shift")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateUpdateShift(Shift shift)
        {
            Shift existing = null;
            if(shift.Id!=default(int))
            {
                existing = await _shiftRepository.GetShift(shift.Id);
                if (User.RoleInPatrol( existing.PatrolId).CanMaintainSchedule()
                    && existing.PatrolId == shift.PatrolId)
                {
                    await _shiftRepository.UpdateShift(shift);
                    return Ok(shift);
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                if (User.RoleInPatrol( shift.PatrolId).CanMaintainSchedule())
                {
                    await _shiftRepository.InsertShift(shift);
                    return Ok(shift);
                }
                else
                {
                    return Forbid();
                }
            }
        }
    }
}
