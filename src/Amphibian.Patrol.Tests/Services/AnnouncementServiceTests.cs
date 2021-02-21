using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Dtos;
using AutoMapper;
using Moq;
using Amphibian.Patrol.Api.Mappings;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Tests.Services
{
    [TestFixture(Category = "Services")]
    public class AnnouncementServiceTests
    {
        private AnnouncementService _announcementService;
        private Mock<IAnnouncementRepository> _announcementRepositoryMock;
        private Mock<IPatrolRepository> _patrolRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailService;
        private IMapper _mapper;
        private Mock<ILogger<EventService>> _loggerMock;
        private Mock<ISystemClock> _systemClockMock;

        [SetUp]
        public void Setup()
        {
            _announcementRepositoryMock = new Mock<IAnnouncementRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _loggerMock = new Mock<ILogger<EventService>>();
            _systemClockMock = new Mock<ISystemClock>();
            _patrolRepositoryMock = new Mock<IPatrolRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailService = new Mock<IEmailService>();

            _announcementService = new AnnouncementService(_announcementRepositoryMock.Object, _loggerMock.Object, _mapper,_systemClockMock.Object,_emailService.Object,_patrolRepositoryMock.Object,_userRepositoryMock.Object);
        }

        [Test]
        public async Task RemovesEventAttributes()
        {
            _announcementRepositoryMock.Setup(x => x.InsertAnnouncement(It.Is<Announcement>(a => !a.AnnouncementHtml.Contains("onmouseout")))).Verifiable();
            await _announcementService.PostAnnouncement(new Announcement()
            {
                AnnouncementMarkdown = @"<div onmouseout=""alert('Gotcha!')""></div>"
            });
        }

        [Test]
        public async Task RemovesScriptTags()
        {
            _announcementRepositoryMock.Setup(x => x.InsertAnnouncement(It.Is<Announcement>(a => !a.AnnouncementHtml.Contains("script")))).Verifiable();
            await _announcementService.PostAnnouncement(new Announcement()
            {
                AnnouncementMarkdown = @"<script>alert('p0wned');</script>"
            });
        }
    }
}
