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
    public class PublicController : ControllerBase
    {
        private readonly ILogger<PublicController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private IPatrolService _patrolService;
        private ISystemClock _clock;
        private IEventRepository _eventRepository;
        private IAnnouncementRepository _announcementRepository;

        public PublicController(ILogger<PublicController> logger, IPatrolRepository patrolRepository,
            IPatrolService patrolService, ISystemClock systemClock, IEventRepository eventRepository, IAnnouncementRepository announcementRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _patrolService = patrolService;
            _clock = systemClock;
            _eventRepository = eventRepository;
            _announcementRepository = announcementRepository;
        }

        [HttpGet]
        [Route("public/patrol/{subdomain}")]
        public async Task<IActionResult> GetPatrolBySubdomain(string subdomain)
        {
            var patrol = await _patrolRepository.GetPatrol(subdomain);
            if(patrol!=null && patrol.EnablePublicSite)
            {
                return Ok(patrol);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("public/events/{subdomain}")]
        public async Task<IActionResult> GetEventsBySubdomain(string subdomain)
        {
            var patrol = await _patrolRepository.GetPatrol(subdomain);
            if (patrol != null && patrol.EnablePublicSite)
            {
                return Ok(patrol);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("public/announcements/{subdomain}")]
        public async Task<IActionResult> GetAnnouncementsBySubdomain(string subdomain)
        {
            var patrol = await _patrolRepository.GetPatrol(subdomain);
            if (patrol != null && patrol.EnablePublicSite)
            {
                return Ok(patrol);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
