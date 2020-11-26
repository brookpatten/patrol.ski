using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class SectionGroup
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int GroupId { get; set; }
    }
}
