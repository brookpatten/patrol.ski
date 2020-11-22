using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class AssignmentProgressByDayDto
    {
        public int AssignmentId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public DateTime Day { get; set; }
        public int requiredsignatures { get; set; }
        public int completedsignatures { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
    }
}
