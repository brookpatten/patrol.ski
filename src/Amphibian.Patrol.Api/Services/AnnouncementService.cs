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

        private ILogger<AnnouncementService> _logger;
        private IMapper _mapper;
        private ISystemClock _clock;
        public AnnouncementService(IAnnouncementRepository announcementRepository, ILogger<AnnouncementService> logger, IMapper mapper, ISystemClock clock)
        {
            _logger = logger;
            _mapper = mapper;
            _announcementRepository = announcementRepository;
            _clock = clock;
        }
        public async Task PostAnnouncement(Announcement announcement)
        {
            

            SanitizeAndNormalize(announcement);
            if (announcement.Id == default(int))
            {
                announcement.CreatedAt = _clock.UtcNow.DateTime;
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
        }
        public async Task PreviewAnnouncement(Announcement announcement)
        {
            SanitizeAndNormalize(announcement);
        }
        public async Task UpdateAnnouncement(Announcement announcement)
        {
            if (!announcement.PostAt.HasValue)
            {
                announcement.PostAt = _clock.UtcNow.DateTime;
            }
            SanitizeAndNormalize(announcement);
            await _announcementRepository.UpdateAnnouncement(announcement);
        }

        private void SanitizeAndNormalize(Announcement announcement)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitized = sanitizer.Sanitize(announcement.AnnouncementMarkdown);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var normalized = Markdown.Normalize(sanitized,pipeline:pipeline);

            var html = Markdown.ToHtml(normalized, pipeline: pipeline);

            announcement.AnnouncementMarkdown = normalized;
            announcement.AnnouncementHtml = html;
        }

        

        public async Task<IEnumerable<Announcement>> GetAnnouncementsForPatrol(int patrolId,bool currentOnly)
        {
            var now = _clock.UtcNow.DateTime;
            var announcements = await _announcementRepository.GetAnnouncements(patrolId, currentOnly ? (DateTime?)now: null);
            return announcements;
        }
    }
}
