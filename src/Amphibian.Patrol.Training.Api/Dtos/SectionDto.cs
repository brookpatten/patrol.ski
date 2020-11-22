using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class SectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<SectionLevelDto> Levels { get; set; }
        public IEnumerable<SectionSkillDto> Skills { get; set; }
        public bool CurrentUserCanSign { get; set; }
    }
}
