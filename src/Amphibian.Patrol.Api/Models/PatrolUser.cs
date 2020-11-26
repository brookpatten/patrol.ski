using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class PatrolUser
    {
        public int Id { get; set; }
        public int PatrolId { get; set; }
        public int UserId { get; set; }
        public Role? Role { get; set; }
    }
}
