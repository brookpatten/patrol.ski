using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class PlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PatrolId { get; set; }
        public IEnumerable<SectionDto> Sections { get; set; }
    }
}
