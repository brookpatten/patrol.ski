using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class Trainee
    {
        public int Id {get;set;}
        public int ScheduledShiftAssignmentId { get; set; }
        public int TraineeUserId { get; set; }
    }
}
