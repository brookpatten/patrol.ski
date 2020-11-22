using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class SectionLevelDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int SectionId { get; set; }
        public Level Level { get; set; }
    }
}
