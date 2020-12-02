using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public enum ShiftStatus
    {
        Assigned,
        Released,
        Claimed
    }
    public class ScheduledShiftAssignment
    {
        public int Id { get; set; }
        public int ScheduledShiftId { get; set; }
        public int AssignedUserId { get; set; }
        public int? ClaimedByUserId { get; set; }
        public int OriginalAssignedUserId { get; set; }
        public ShiftStatus Status { get; set; }
    }
}
