using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public class SysAdminService : ISysAdminService
    {
        public string[] _systemAdministrators;

        public SysAdminService(AppConfiguration configuration)
        {
            _systemAdministrators = configuration.SystemAdministrators;
        }
        public bool IsUserSysAdmin(UserIdentifier user)
        {
            return _systemAdministrators.Any(x => x == user.Email);
        }
    }
}
