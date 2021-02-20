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
    public class EventRepositoryTests : DatabaseConnectedTestFixture
    {
        private EventRepository _eventRepository;

        [SetUp]
        public void SetUp()
        {
            _eventRepository = new EventRepository(_connection);
        }

        [Test]
        public async Task CanInsertEvents()
        {
            var patrolEvent = new Event()
            {
                PatrolId = 1,
                Name="Hello world",
                Location="World",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                StartsAt =new DateTime(2001,1,1,0,0,0),
                EndsAt= new DateTime(2001, 1, 1,23,59,59)
            };

            await _eventRepository.InsertEvent(patrolEvent);

            Assert.NotZero(patrolEvent.Id);
        }

        [Test]
        public async Task CanRetrieveInsertedEvent()
        {
            var patrolEvent = new Event()
            {
                PatrolId = 1,
                Name = "Hello world",
                Location = "World",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                StartsAt = new DateTime(2001, 1, 1, 0, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 23, 59, 59)
            };

            await _eventRepository.InsertEvent(patrolEvent);

            var events = await _eventRepository.GetEvents(1, new DateTime(2001, 1, 1, 0, 0, 0), new DateTime(2001, 1, 1, 23, 59, 59),true,true);

            Assert.IsNotEmpty(events);
        }

        [Test]
        public async Task CanUpdateEvent()
        {
            var patrolEvent = new Event()
            {
                PatrolId = 1,
                Name = "Hello world",
                Location = "World",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                StartsAt = new DateTime(2001, 1, 1, 0, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 23, 59, 59)
            };

            await _eventRepository.InsertEvent(patrolEvent);

            patrolEvent.Name = "Test";

            await _eventRepository.UpdateEvent(patrolEvent);

            var events = await _eventRepository.GetEvents(1, new DateTime(2001, 1, 1, 0, 0, 0), new DateTime(2001, 1, 1, 23, 59, 59), true, true);

            Assert.AreEqual(patrolEvent.Name, events.ToList()[0].Name);
        }

        [Test]
        public async Task EventsBeforeRangeNotReturned()
        {
            var patrolEvent = new Event()
            {
                PatrolId = 1,
                Name = "Hello world",
                Location = "World",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                StartsAt = new DateTime(2001, 1, 1, 0, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 23, 59, 59)
            };

            await _eventRepository.InsertEvent(patrolEvent);

            var events = await _eventRepository.GetEvents(1, new DateTime(2002, 1, 1, 0, 0, 0), new DateTime(2002, 1, 1, 23, 59, 59), true, true);

            Assert.IsEmpty(events);
        }

        [Test]
        public async Task EventsAfterRangeNotReturned()
        {
            var patrolEvent = new Event()
            {
                PatrolId = 1,
                Name = "Hello world",
                Location = "World",
                CreatedAt = DateTime.Now,
                CreatedByUserId = 1,
                StartsAt = new DateTime(2001, 1, 1, 0, 0, 0),
                EndsAt = new DateTime(2001, 1, 1, 23, 59, 59)
            };

            await _eventRepository.InsertEvent(patrolEvent);

            var events = await _eventRepository.GetEvents(1, new DateTime(2000, 1, 1, 0, 0, 0), new DateTime(2000, 1, 1, 23, 59, 59), true, true);

            Assert.IsEmpty(events);
        }
    }
}
