using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Amphibian.Patrol.Training.Api.Extensions
{
    public static class HelperExtensionMethods
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var id = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.Parse(id);
        }
    }
}
