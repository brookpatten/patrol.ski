using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Infrastructure
{
    public class ApiLogMiddleware
    {
        private readonly RequestDelegate _next;
        
        private ISystemClock _clock;

        public ApiLogMiddleware(RequestDelegate next, ISystemClock clock)
        {
            _next = next;
            _clock = clock;
        }
        public async Task Invoke(HttpContext httpContext, IApiLogRepository apiLogRepository)
        {
            Stopwatch watch = new Stopwatch();
            var now = _clock.UtcNow.UtcDateTime;
            watch.Start();
            await _next(httpContext);
            watch.Stop();

            var user = httpContext.User.ParseAllClaims();

            var log = new ApiLog();
            log.DurationMs = (int)watch.ElapsedMilliseconds;
            log.Route = httpContext.GetRouteData().Values["controller"] + "." + httpContext.GetRouteData().Values["action"];
            log.StartedAt = now;
            log.UserId = user != null ? (int?)user.User.Id : null;
            log.Verb = httpContext.Request.Method;
            log.ResponseCode = httpContext.Response.StatusCode;
            log.QueryString = httpContext.Request.QueryString.ToString();

            try
            {
                await apiLogRepository.InsertApiLog(log);
            }
            catch(Exception ex)
            {

            }
        }
    }
}