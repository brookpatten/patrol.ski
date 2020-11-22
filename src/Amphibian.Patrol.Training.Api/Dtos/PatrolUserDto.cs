using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class PatrolUserDto
    {
        public int Id { get; set; }
        public int PatrolUserId { get; set; }
        public int PatrolId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role? Role { get; set; }
        public IList<Group> Groups { get; set; }
    }
}
