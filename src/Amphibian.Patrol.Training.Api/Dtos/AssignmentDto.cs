using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public IEnumerable<SignatureDto> Signatures { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
