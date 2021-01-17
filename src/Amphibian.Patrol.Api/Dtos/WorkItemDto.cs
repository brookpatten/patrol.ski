using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class WorkItemDto:WorkItem
    {
        public RecurringWorkItem RecurringWorkItem { get; set; }
        public ScheduledShiftAssignmentDto ScheduledShift { get; set; }
        public Group AdminGroup { get; set; }
        public UserIdentifier CreatedBy { get; set; }
        public UserIdentifier CanceledBy { get; set; }
        public UserIdentifier CompletedBy { get; set; }
        public List<WorkItemAssignmentDto> Assignments { get; set; }
        /// <summary>
        /// can the current user perform completion on this work item (via mode+ assignments)
        /// </summary>
        public bool CanComplete { get; set; }
        /// <summary>
        /// can the current user perform admin (force complete, cancel) on this work item (via createdby + admin group membership)
        /// </summary>
        public bool CanAdmin { get; set; }
        public bool IsDue { get; set; }
    }

    public class WorkItemAssignmentDto: WorkItemAssignment
    {
        public UserIdentifier User { get; set; }
    }
}
