using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IAnnouncementRepository
    {
        Task<IEnumerable<Announcement>> GetAnnouncements(int patrolId, DateTime? now, bool isInternal, bool isPublic);
        Task InsertAnnouncement(Announcement announcement);
        Task UpdateAnnouncement(Announcement announcement);
        Task<Announcement> GetById(int id);
    }
}
