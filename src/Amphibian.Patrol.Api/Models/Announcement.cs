using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Subject { get; set; }
        public string AnnouncementMarkdown { get; set; }
        public string AnnouncementHtml { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PostAt { get; set; }
        public DateTime? ExpireAt { get; set; }
        public bool Emailed { get; set; }
    }
}
