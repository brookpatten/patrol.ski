using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IAnnouncementService
    {
        Task PostAnnouncement(Announcement announcement);
        Task PreviewAnnouncement(Announcement announcement);
        Task<IEnumerable<Announcement>> GetAnnouncementsForPatrol(int patrolId, bool currentOnly, bool isInternal, bool isPublic);
    }
}
