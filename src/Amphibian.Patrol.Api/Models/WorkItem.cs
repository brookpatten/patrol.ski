using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public enum CompletionMode { Any, AnyAssigned, AllAssigned, AdminOnly}
    

    public class WorkItem
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public int? RecurringWorkItemId { get; set; }
        public int? ScheduledShiftId { get; set; }
        public string Name { get; set; }
        public string DescriptionMarkup { get; set; }
        public string Location { get; set; }
        public DateTime ScheduledAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? CompletedAt { get; set; }

        public CompletionMode CompletionMode { get; set; }
        public int? AdminGroupId { get; set; }
    }
}
