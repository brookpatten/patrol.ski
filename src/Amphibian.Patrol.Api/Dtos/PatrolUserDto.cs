using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class PatrolUserDto:UserIdentifier
    {
        public int PatrolUserId { get; set; }
        public int PatrolId { get; set; }
        public Role? Role { get; set; }
        public IList<Group> Groups { get; set; }
    }
}
