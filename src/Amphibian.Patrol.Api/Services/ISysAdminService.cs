using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface ISysAdminService
    {
        public bool IsUserSysAdmin(UserIdentifier user);
    }
}
