using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Serilog.Configuration;
using Microsoft.CodeAnalysis.CSharp;
using Markdig;
using Microsoft.AspNetCore.Authentication;
using Markdig.Renderers.Normalize;
using Ganss.XSS;

namespace Amphibian.Patrol.Api.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private IAnnouncementRepository _announcementRepository;

        private ILogger<EventService> _logger;
        private IMapper _mapper;
        private ISystemClock _clock;
        private IEmailService _emailService;
        private IPatrolRepository _patrolRepository;
        private IUserRepository _userRepository;
        public AnnouncementService(IAnnouncementRepository announcementRepository, ILogger<EventService> logger, IMapper mapper, 
            ISystemClock clock, IEmailService emailService, IPatrolRepository patrolRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _announcementRepository = announcementRepository;
            _clock = clock;
            _userRepository = userRepository;
            _patrolRepository = patrolRepository;
            _emailService = emailService;
        }
        public async Task PostAnnouncement(Announcement announcement)
        {
            var plainText = SanitizeAndNormalize(announcement);
            if (announcement.Id == default(int))
            {
                announcement.CreatedAt = _clock.UtcNow.UtcDateTime;
                if (!announcement.PostAt.HasValue || announcement.PostAt == default(DateTime))
                {
                    announcement.PostAt = announcement.CreatedAt;
                }
                await _announcementRepository.InsertAnnouncement(announcement);
            }
            else
            {
                await _announcementRepository.UpdateAnnouncement(announcement);
            }

            var createdBy = await _userRepository.GetUser(announcement.CreatedByUserId);
            var patrol = await _patrolRepository.GetPatrol(announcement.PatrolId);

            if(announcement.Emailed)
            {
                var users = (await _patrolRepository.GetUsersForPatrol(announcement.PatrolId)).ToList();
                await _emailService.SendAnnouncementEmail(createdBy,users,patrol.Name,announcement.Subject, plainText, announcement.AnnouncementHtml);
            }
        }
        public async Task PreviewAnnouncement(Announcement announcement)
        {
            SanitizeAndNormalize(announcement);
        }
        
        private string SanitizeAndNormalize(Announcement announcement)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitized = sanitizer.Sanitize(announcement.AnnouncementMarkdown);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var normalized = Markdown.Normalize(sanitized,pipeline:pipeline);

            var html = Markdown.ToHtml(normalized, pipeline: pipeline);

            announcement.AnnouncementMarkdown = normalized;
            announcement.AnnouncementHtml = html;

            var plainText = Markdown.ToPlainText(html);
            return plainText;
        }

        

        public async Task<IEnumerable<Announcement>> GetAnnouncementsForPatrol(int patrolId,bool currentOnly, bool isInternal, bool isPublic)
        {
            var now = _clock.UtcNow.UtcDateTime;
            var announcements = await _announcementRepository.GetAnnouncements(patrolId, currentOnly ? (DateTime?)now: null,isInternal,isPublic);
            return announcements;
        }
    }
}
