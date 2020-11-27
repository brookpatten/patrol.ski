using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}
