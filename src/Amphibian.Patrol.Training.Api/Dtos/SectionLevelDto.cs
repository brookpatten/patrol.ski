using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class SectionLevelDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public int Order { get; set; }
    }
}
