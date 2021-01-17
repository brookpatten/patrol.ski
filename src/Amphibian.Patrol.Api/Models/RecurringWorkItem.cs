using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public enum RecurInterval { Year,Month,Week,Day,Hour,Minute}
    public class RecurringWorkItem
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public string Name { get; set; }
        public string DescriptionMarkup { get; set; }
        public string Location { get; set; }

        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public CompletionMode CompletionMode { get; set; }
        public int? MaximumRandomCount { get; set; }
        public int? AdminGroupId { get; set; }

        public DateTime? RecurStart { get; set; }
        public DateTime? RecurEnd { get; set; }
        public RecurInterval? RecurInterval { get; set; }
        public int? RecurIntervalCount { get; set; }
    }
}
