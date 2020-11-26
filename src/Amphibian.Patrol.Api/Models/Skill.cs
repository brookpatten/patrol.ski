using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
