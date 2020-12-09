using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    /// <summary>
    /// the table name for timeentry is "timeentrys" due to dommel defaults not understanding ies
    /// and i'm too lazy to implement a custom table name resolver, I suppose I'll deal with until I can't.
    /// </summary>
    public class TimeEntry
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public int UserId { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public int? DurationSeconds { get; set; }
        public DateTime? MostRecentReminderSentAt { get; set; }
    }
}
