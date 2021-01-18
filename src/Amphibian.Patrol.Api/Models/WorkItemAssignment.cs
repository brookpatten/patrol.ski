using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class WorkItemAssignment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkItemId { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string WorkNotes { get; set; }
    }
}
