using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class CurrentTimeEntryDto
    {
        public UserIdentifier User { get; set; }
        public TimeEntry TimeEntry { get; set; }
        public ScheduledShift ScheduledShift { get; set; }
        public TimeEntryScheduledShiftAssignment TimeEntryScheduledShiftAssignment { get; set; }
        public Shift Shift { get; set; }
        public Group Group { get; set; }
    }
}
