using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class SectionLevel
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int LevelId { get; set; }
        public int Order { get; set; }
    }
}
