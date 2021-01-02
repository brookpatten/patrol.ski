using System;
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
    public class WorkItemController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPatrolService _patrolService;
        private readonly IWorkItemService _workItemService;
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _clock;

        public WorkItemController(ILogger<ScheduleController> logger, IWorkItemService workItemService, IPatrolRepository patrolRepository, ISystemClock clock, 
            IPatrolService patrolService, IUserRepository userRepository, IWorkItemRepository workItemRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _clock = clock;
            _patrolService = patrolService;
            _userRepository = userRepository;
            _workItemService = workItemService;
            _workItemRepository = workItemRepository;
        }

        [HttpGet]
        [Route("workitem/{id}")]
        [Authorize]
        public async Task<IActionResult> GetWorkItem(int id)
        {
            var workItem = await _workItemService.GetWorkItem(id);
            if (User.IsInPatrol(workItem.PatrolId))
            {
                return Ok(workItem);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("workitem")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> SaveWorkItem(WorkItemDto workItem)
        {
            if (User.RoleInPatrol(workItem.PatrolId).CanMaintainWorkItems())
            {
                if(workItem.Id!=default(int))
                {
                    var existing = await _workItemService.GetWorkItem(workItem.Id);
                    if(!User.RoleInPatrol(existing.PatrolId).CanMaintainWorkItems())
                    {
                        return Forbid();
                    }
                }
                await _workItemService.SaveWorkItem(workItem, User.UserId());
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("workitem/recurring/{id}")]
        [Authorize]
        public async Task<IActionResult> GetRecurringWorkItem(int id)
        {
            var workItem = await _workItemService.GetRecurringWorkItem(id);
            if (User.RoleInPatrol(workItem.PatrolId).CanMaintainWorkItems())
            {
                return Ok(workItem);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("workitem/recurring")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> SaveRecurringWorkItem(RecurringWorkItemDto workItem)
        {
            if (User.RoleInPatrol(workItem.PatrolId).CanMaintainWorkItems())
            {
                if (workItem.Id != default(int))
                {
                    var existing = await _workItemService.GetRecurringWorkItem(workItem.Id);
                    if (!User.RoleInPatrol(existing.PatrolId).CanMaintainWorkItems())
                    {
                        return Forbid();
                    }
                }
                await _workItemService.SaveRecurringWorkItem(workItem, User.UserId());
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }

        public class CompleteWorkItemDto
        {
            public int Id { get; set; }
            public string WorkNotes { get; set; }
        }
        [HttpPost]
        [Route("workitem/complete")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CompleteWorkItem(CompleteWorkItemDto dto)
        {
            var workItem = await _workItemService.GetWorkItem(dto.Id);
            if (User.IsInPatrol(workItem.PatrolId))
            {
                if(await _workItemService.CanCompleteWorkItem(dto.Id, User.UserId()))
                {
                    await _workItemService.CompleteWorkItem(dto.Id, User.UserId(), dto.WorkNotes);
                    return Ok();
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

        [HttpPost]
        [Route("workitem/cancel")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CancelWorkItem(CompleteWorkItemDto dto)
        {
            var workItem = await _workItemService.GetWorkItem(dto.Id);
            if (User.IsInPatrol(workItem.PatrolId))
            {
                if (await _workItemService.CanCancelWorkItem(dto.Id, User.UserId()))
                {
                    await _workItemService.CancelWorkItem(dto.Id, User.UserId());
                    return Ok();
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

        /// <summary>
        /// return current work items for current user in a given patrol
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("workitem/current/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> CurrentForPatrol(int patrolId)
        {
            if (User.IsInPatrol(patrolId))
            {
                var now = _clock.UtcNow.UtcDateTime;
                var workItems = await _workItemRepository.GetWorkItems(User.UserId(),patrolId, complete:false,scheduledBefore:now, completableByUserId:User.UserId());
                return Ok(workItems);
            }
            else
            {
                return Forbid();
            }
        }

        public class WorkItemSearchDto
        {
            public int PatrolId { get; set; }
            public bool? Complete { get; set; }
            public int? CompletedByUserId { get; set; }
            public int? RecurringWorkItemId { get; set; }
            public DateTime? ScheduledBefore { get; set; }
            public DateTime? ScheduledAfter { get; set; }
            public int? ShiftId { get; set; }
            public int? AdminGroupId { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }
        [HttpPost]
        [Route("workitem/search")]
        [Authorize]
        public async Task<IActionResult> WorkItemSearch(WorkItemSearchDto dto)
        {
            if (User.IsInPatrol(dto.PatrolId))
            {
                var workItems = await _workItemRepository.GetWorkItems(User.UserId(), dto.PatrolId, dto.Complete, dto.CompletedByUserId, dto.RecurringWorkItemId, dto.ScheduledBefore, dto.ScheduledAfter, dto.ShiftId, dto.AdminGroupId, dto.Name, dto.Location);
                return Ok(workItems);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
