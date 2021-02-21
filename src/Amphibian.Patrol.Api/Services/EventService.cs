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
    public class EventService : IEventService
    {
        private IEventRepository _eventRepository;

        private ILogger<EventService> _logger;
        private IMapper _mapper;
        private ISystemClock _clock;
        private IEmailService _emailService;
        private IPatrolRepository _patrolRepository;
        private IUserRepository _userRepository;
        public EventService(IEventRepository eventRepository, ILogger<EventService> logger, IMapper mapper,
            ISystemClock clock, IEmailService emailService, IPatrolRepository patrolRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _clock = clock;
            _userRepository = userRepository;
            _patrolRepository = patrolRepository;
            _emailService = emailService;
        }
        public async Task PostEvent(Event theEvent, int userId)
        {
            var plainText = SanitizeAndNormalize(theEvent);
            if (theEvent.Id == default(int))
            {
                theEvent.CreatedByUserId = userId;
                theEvent.CreatedAt = _clock.UtcNow.UtcDateTime;
                await _eventRepository.InsertEvent(theEvent);
            }
            else
            {
                theEvent.CreatedByUserId = userId;
                await _eventRepository.UpdateEvent(theEvent);
            }

            var createdBy = await _userRepository.GetUser(theEvent.CreatedByUserId);
            var patrol = await _patrolRepository.GetPatrol(theEvent.PatrolId);

            if (theEvent.Emailed)
            {
                var users = (await _patrolRepository.GetUsersForPatrol(theEvent.PatrolId)).ToList();
                await _emailService.SendEventEmail(createdBy, users, patrol, theEvent.Name, theEvent.Location, plainText, theEvent.EventHtml, theEvent.StartsAt, theEvent.EndsAt);
            }
        }

        private string SanitizeAndNormalize(Event announcement)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitized = sanitizer.Sanitize(announcement.EventMarkdown);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var normalized = Markdown.Normalize(sanitized, pipeline: pipeline);

            var html = Markdown.ToHtml(normalized, pipeline: pipeline);

            announcement.EventMarkdown = normalized;
            announcement.EventHtml = html;

            var plainText = Markdown.ToPlainText(html);
            return plainText;
        }
    }
}
