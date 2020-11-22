using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class AssignmentHeaderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int SignaturesRequired { get; set; }
        public int Signatures { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? DueAt { get; set; }
    }
}
