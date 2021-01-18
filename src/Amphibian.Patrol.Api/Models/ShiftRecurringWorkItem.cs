using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public enum ShiftAssignmentMode { CopyPrevious, ManualEachShift, Auto }
    public class ShiftRecurringWorkItem
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int RecurringWorkItemId { get; set; }
        public int ScheduledAtHour { get; set; }
        public int ScheduledAtMinute { get; set; }
        public ShiftAssignmentMode? ShiftAssignmentMode { get; set; }
    }
}
