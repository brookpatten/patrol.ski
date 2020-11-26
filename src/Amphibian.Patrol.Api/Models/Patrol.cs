using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class Patrol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool EnableTraining { get; set; }
        public bool EnableAnnouncements { get; set; }
    }
}
