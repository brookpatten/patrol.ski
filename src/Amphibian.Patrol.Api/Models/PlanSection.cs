using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class PlanSection
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int SectionId { get; set; }
    }
}
