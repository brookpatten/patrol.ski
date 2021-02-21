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
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IEventRepository _eventRepository;
        private IPatrolService _patrolService;
        private ISystemClock _clock;
        private IEventService _eventService;

        public EventController(ILogger<EventController> logger, IPatrolRepository patrolRepository,
            IPatrolService patrolService, ISystemClock systemClock, IEventRepository eventRepository, IEventService eventService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolService = patrolService;
            _clock = systemClock;
            _eventRepository = eventRepository;
            _eventService = eventService;
        }

        public class EventQuery
        {
            public int PatrolId { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public bool IsPublic { get; set; }
            public bool IsInternal { get; set; }
        }
        [HttpPost]
        [Route("events/search")]
        [Authorize]
        public async Task<IActionResult> GetEvents(EventQuery query)
        {
            if (User.PatrolIds().Any(x=>x== query.PatrolId))
            {
                var patrolEvents = await _eventRepository.GetEvents(query.PatrolId, query.From, query.To,query.IsInternal,query.IsPublic);
                return Ok(patrolEvents);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("events/upcoming/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetUpcomingEvents(int patrolId)
        {
            if (User.PatrolIds().Any(x => x == patrolId))
            {
                var patrolEvents = await _eventRepository.GetUpcomingEvents(patrolId, _clock.UtcNow.UtcDateTime,true,false);
                return Ok(patrolEvents);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("event/{id}")]
        [Authorize]
        public async Task<IActionResult> GetEvent(int id)
        {
            var patrolEvent = await _eventRepository.GetById(id);
            
            if (User.PatrolIds().Any(x => x == patrolEvent.PatrolId))
            {
                return Ok(patrolEvent);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("events")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> PostEvent(Event patrolEvent)
        {
            if (User.RoleInPatrol( patrolEvent.PatrolId).CanMaintainEvents())
            {
                if (patrolEvent.Id != default(int))
                {
                    var existing = await _eventRepository.GetById(patrolEvent.Id);
                    if (existing.PatrolId != patrolEvent.PatrolId)
                    {
                        return Forbid();
                    }
                }

                await _eventService.PostEvent(patrolEvent,User.UserId());

                return Ok(patrolEvent);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpDelete]
        [Route("event/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var existing = await _eventRepository.GetById(id);

            if (User.RoleInPatrol( existing.PatrolId).CanMaintainEvents())
            {
                await _eventRepository.Delete(existing);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
    }
}