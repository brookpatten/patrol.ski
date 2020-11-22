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
        public string Name { get; set; }
        public Role? Role { get; set; }
        public IList<Permission> Permissions
        {
            get
            {
                return this.Role.Permissions();
            }
        }
    }
}
