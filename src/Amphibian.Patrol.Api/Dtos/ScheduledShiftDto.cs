using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class ScheduledShiftDto
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public Shift Shift { get; set; }
        public Group Group { get; set; }
        public List<ScheduledShiftAssignmentDto> Assignments { get; set; }
    }
}
