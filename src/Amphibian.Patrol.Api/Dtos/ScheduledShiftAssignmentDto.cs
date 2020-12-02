using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class ScheduledShiftAssignmentDto
    {
        public int Id { get; set; }
        public int ScheduledShiftId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public Shift Shift { get; set; }
        public Group Group { get; set; }
        public UserIdentifier AssignedUser { get; set; }
        public UserIdentifier ClaimedByUser { get; set; }
        public UserIdentifier OriginalAssignedUser { get; set; }
        public int TraineeCount { get; set; }
        public ShiftStatus Status { get; set; }
    }
}
