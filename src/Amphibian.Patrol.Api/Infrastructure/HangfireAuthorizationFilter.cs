using Amphibian.Patrol.Api.Dtos;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Infrastructure
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            if(httpContext.User.Identity.IsAuthenticated)
            {
                var claims = httpContext.User.ParseAllClaims();
                return claims.IsSysAdmin.HasValue && claims.IsSysAdmin.Value;
            }
            else
            {
                return false;
            }
        }
    }
}
