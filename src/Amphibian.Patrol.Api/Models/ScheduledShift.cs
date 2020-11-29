using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class ScheduledShift
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int? ShiftId { get; set; }
        public int? GroupId { get; set; }
    }
}
