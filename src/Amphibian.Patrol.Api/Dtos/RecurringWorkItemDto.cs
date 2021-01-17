using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class RecurringWorkItemDto: RecurringWorkItem
    {
        public List<ShiftRecurringWorkItemDto> Shifts { get; set; }
        public Group AdminGroup { get; set; }
        public UserIdentifier CreatedBy { get; set; }
        public List<UserIdentifier> NextOccurenceUsers { get; set; }
        public int CompletedWorkItemCount { get; set; }
        public int WorkItemCount { get; set; }
        public DateTime? FirstScheduledAt { get; set; }
        public DateTime? LastScheduledAt { get; set; }
        public DateTime? NextScheduledAt { get; set; }
    }

    public class ShiftRecurringWorkItemDto:ShiftRecurringWorkItem
    {
        public Shift Shift { get; set; }
    }
}
