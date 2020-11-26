using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Dommel;
using System.Linq;
using System.Runtime.InteropServices;
using AutoMapper;
using Amphibian.Patrol.Api.Mappings;

namespace Amphibian.Patrol.Tests.Repositories
{
    public class AnnouncementRepositoryTests : DatabaseConnectedTestFixture
    {
        private AnnouncementRepository _announcementRepository;

        [SetUp]
        public void SetUp()
        {
            _announcementRepository = new AnnouncementRepository(_connection);
        }

        [Test]
        public async Task CanInsertAnnouncements()
        {
            var announcement = new Announcement()
            {
                PatrolId = 1,
                AnnouncementMarkdown="Hello world",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                PostAt = DateTime.Now,
                Subject="Test"
            };

            await _announcementRepository.InsertAnnouncement(announcement);

            Assert.NotZero(announcement.Id);
        }

        [Test]
        public async Task CanRetrieveInsertedAnnouncement()
        {
            var announcement = new Announcement()
            {
                PatrolId = 1,
                AnnouncementMarkdown = "Hello world",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                PostAt = DateTime.Now,
                Subject = "Test"
            };

            await _announcementRepository.InsertAnnouncement(announcement);

            var announcements = await _announcementRepository.GetAnnouncements(1,null);

            Assert.IsNotEmpty(announcements);
        }

        [Test]
        public async Task CanUpdateAnnouncement()
        {
            var announcement = new Announcement()
            {
                PatrolId = 1,
                AnnouncementMarkdown = "Hello world",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                PostAt = DateTime.Now,
                Subject = "Test"
            };

            await _announcementRepository.InsertAnnouncement(announcement);

            announcement.AnnouncementMarkdown = "Test";

            await _announcementRepository.UpdateAnnouncement(announcement);

            var announcements = await _announcementRepository.GetAnnouncements(1, null);

            Assert.AreEqual(announcement.AnnouncementMarkdown, announcements.ToList()[0].AnnouncementMarkdown);
        }

        [Test]
        public async Task AnnouncementsNotPostedYetAreNotReturned()
        {
            DateTime now = new DateTime(2001, 1, 1);
            DateTime postAt = new DateTime(2001, 1, 2);

            var announcement = new Announcement()
            {
                PatrolId = 1,
                AnnouncementMarkdown = "Hello world",
                CreatedAt = now,
                CreatedByUserId = 1,
                PostAt = postAt,
                Subject = "Test"
            };

            await _announcementRepository.InsertAnnouncement(announcement);

            var announcements = await _announcementRepository.GetAnnouncements(1, now);

            Assert.IsEmpty(announcements);
        }

        [Test]
        public async Task AnnouncementsExpiredAreNotReturned()
        {
            DateTime now = new DateTime(2001, 1, 3);
            DateTime createAt = new DateTime(2001, 1, 1);
            DateTime expireAt = new DateTime(2001, 1, 2);

            var announcement = new Announcement()
            {
                PatrolId = 1,
                AnnouncementMarkdown = "Hello world",
                CreatedAt = createAt,
                CreatedByUserId = 1,
                PostAt = createAt,
                Subject = "Test",
                ExpireAt = expireAt
            };

            await _announcementRepository.InsertAnnouncement(announcement);

            var announcements = await _announcementRepository.GetAnnouncements(1, now);

            Assert.IsEmpty(announcements);
        }
    }
}
