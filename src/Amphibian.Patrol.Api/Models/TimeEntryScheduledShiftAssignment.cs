using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class TimeEntryScheduledShiftAssignment
    {
        public int Id { get; set; }
        public int TimeEntryId { get; set; }
        public int ScheduledShiftAssignmentId { get; set; }
        public int DurationSeconds { get; set; }
    }
}
