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
        public List<WorkItemAssignmentDto> Assignments { get; set; }
    }

    public class WorkItemAssignmentDto: WorkItemAssignment
    {
        public UserIdentifier User { get; set; }
    }
}
