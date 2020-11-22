using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class AssignmentCountByPlanByDayDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanColor { get; set; }
        public List<AssignmentCountByDayDto> CountsByDay { get; set; }
    }
    public class AssignmentCountByDayDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanColor { get; set; }
        public DateTime Day { get; set; }
        public int OpenAssignmentCount { get; set; }
    }
}
