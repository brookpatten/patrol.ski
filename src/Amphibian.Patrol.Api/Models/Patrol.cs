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
        public bool EnableEvents { get; set; }
        public bool EnableScheduling { get; set; }
        public bool EnableShiftSwaps { get; set; }
        public bool EnableTimeClock { get; set; }
        public string TimeZone { get; set; }
    }
}
