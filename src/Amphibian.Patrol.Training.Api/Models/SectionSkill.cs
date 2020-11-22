using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class SectionSkill
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int SkillId { get; set; }
        public int RowIndex { get; set; }
    }
}
