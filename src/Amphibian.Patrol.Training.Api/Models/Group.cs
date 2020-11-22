using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public string Name { get; set; }
    }
}
