using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class SysAdminController : ControllerBase
    {
        private ISysAdminService _sysAdminService;
        private IApiLogRepository _apiLogRepository;
        ILogger<SysAdminController> _logger;
        private IUserRepository _userRepository;

        public SysAdminController(ILogger<SysAdminController> logger,ISysAdminService sysAdminService, IApiLogRepository apiLogRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _sysAdminService = sysAdminService;
            _apiLogRepository = apiLogRepository;
            _userRepository = userRepository;
        }


        public class MetricsQuery
        {
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public int? UserId { get; set; }
            public string Route { get; set; }
        }

        [HttpPost]
        [Route("sysadmin/metrics/routes")]
        [Authorize]
        public async Task<IActionResult> GetRoutes(MetricsQuery query)
        {
            var user = await _userRepository.GetUser(User.ParseAllClaims().User.Id);
            if (_sysAdminService.IsUserSysAdmin(user))
            {
                var routes = await _apiLogRepository.SearchApiLogs(query.From, query.To, query.UserId, query.Route);

                var grouped = routes.GroupBy(x => x.Route)
                    .Select(x => new { Route = x.Key, Count = x.Count() })
                    .OrderBy(x=>x.Count)
                    .ToList();

                return Ok(grouped);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("sysadmin/metrics/users")]
        [Authorize]
        public async Task<IActionResult> GetUsers(MetricsQuery query)
        {
            var user = await _userRepository.GetUser(User.ParseAllClaims().User.Id);
            if (_sysAdminService.IsUserSysAdmin(user))
            {
                var routes = await _apiLogRepository.SearchApiLogs(query.From, query.To, query.UserId, query.Route);

                var grouped = routes.GroupBy(x => x.UserId)
                    .Select(x => new { User = x.First().User, Count = x.Count() })
                    .OrderBy(x=>x.Count)
                    .ToList();

                return Ok(grouped);
            }
            else
            {
                return Forbid();
            }
        }

        public class DateCount
        {
            public DateTime Day { get; set; }
            public int Count { get; set; }
        }
        [HttpPost]
        [Route("sysadmin/metrics/days")]
        [Authorize]
        public async Task<IActionResult> GetDays(MetricsQuery query)
        {
            var user = await _userRepository.GetUser(User.ParseAllClaims().User.Id);
            if (_sysAdminService.IsUserSysAdmin(user))
            {
                var routes = await _apiLogRepository.SearchApiLogs(query.From, query.To, query.UserId, query.Route);

                var grouped = routes.GroupBy(x => new { x.StartedAt.Year,x.StartedAt.Month,x.StartedAt.Day })
                    .Select(x => new DateCount { Day = new DateTime(x.First().StartedAt.Year,x.First().StartedAt.Month,x.First().StartedAt.Day), Count = x.Count() })
                    .ToList();

                for(var x = query.From;x<query.To;x=x+new TimeSpan(1,0,0,0))
                {
                    if(!grouped.Any(y=>y.Day.Year==x.Value.Year && y.Day.Month == x.Value.Month && y.Day.Day == x.Value.Day))
                    {
                        grouped.Add(new DateCount() { Day = x.Value, Count = 0 });
                    }
                }

                grouped = grouped.OrderBy(x => x.Day).ToList();

                return Ok(grouped);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("sysadmin/performance/routes")]
        [Authorize]
        public async Task<IActionResult> GetRouteMetrics(MetricsQuery query)
        {
            var user = await _userRepository.GetUser(User.ParseAllClaims().User.Id);
            if (_sysAdminService.IsUserSysAdmin(user))
            {
                var metrics = await _apiLogRepository.GetRouteMetrics(query.From, query.To, query.UserId, query.Route);
                return Ok(metrics);
            }
            else
            {
                return Forbid();
            }
        }
    }
}
