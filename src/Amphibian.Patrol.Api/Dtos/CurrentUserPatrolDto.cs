using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    /// <summary>
    /// used to return what access current user has to a given patrol
    /// </summary>
    public class CurrentUserPatrolDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool EnableTraining { get; set; }
        public bool EnableAnnouncements { get; set; }
        public bool EnableEvents { get; set; }
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
