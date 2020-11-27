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
    public class AnnouncementController : ControllerBase
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IAnnouncementService _announcementService;
        private readonly IAnnouncementRepository _announcementRepository;
        private IPatrolService _patrolService;
        private ISystemClock _clock;

        public AnnouncementController(ILogger<AnnouncementController> logger, IPatrolRepository patrolRepository, IAnnouncementService announcementService,
            IPatrolService patrolService, ISystemClock systemClock, IAnnouncementRepository announcementRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _announcementService = announcementService;
            _patrolService = patrolService;
            _clock = systemClock;
            _announcementRepository = announcementRepository;
        }

        [HttpGet]
        [Route("announcements/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetAnnouncements(int patrolId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            if (patrols.Any(x=>x.Id==patrolId))
            {
                var announcements = await _announcementService.GetAnnouncementsForPatrol(patrolId,false);
                return Ok(announcements);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("announcements/current/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentAnnouncements(int patrolId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            if (patrols.Any(x => x.Id == patrolId))
            {
                var announcements = await _announcementService.GetAnnouncementsForPatrol(patrolId,true);
                return Ok(announcements);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("announcement/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAnnouncement(int id)
        {
            var announcement = await _announcementRepository.GetById(id);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());

            if (patrols.Any(x => x.Id == announcement.PatrolId))
            {
                return Ok(announcement);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("announcements")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> PostAnnouncement(Announcement announcement)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), announcement.PatrolId)).CanMaintainAnnouncements())
            {
                announcement.CreatedByUserId = User.GetUserId();

                if(announcement.Id!=default(int))
                {
                    var existing = await _announcementRepository.GetById(announcement.Id);
                    if(existing.PatrolId!=announcement.PatrolId)
                    {
                        return Forbid();
                    }
                }

                await _announcementService.PostAnnouncement(announcement);
                return Ok(announcement);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("announcements/expire/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ExpireAnnouncement(int id)
        {
            var announcement = await _announcementRepository.GetById(id);
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), announcement.PatrolId)).CanMaintainAnnouncements())
            {
                announcement.ExpireAt = _clock.UtcNow.DateTime;
                await _announcementService.PostAnnouncement(announcement);
                return Ok(announcement);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("announcements/preview")]
        [Authorize]
        public async Task<IActionResult> PreviewAnnouncement(Announcement announcement)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), announcement.PatrolId)).CanMaintainAnnouncements())
            {
                await _announcementService.PreviewAnnouncement(announcement);
                return Ok(announcement);
            }
            else
            {
                return Forbid();
            }
        }
    }
}