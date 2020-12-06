using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class SystemController : ControllerBase
    {
        [HttpGet]
        [Route("timezones")]
        public async Task<IActionResult> GetTimezones()
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones().OrderBy(x=>x.StandardName).Select(x => new { value = x.Id, label = x.StandardName, offset = x.BaseUtcOffset.ToString() });
            return Ok(timezones);
        }
    }
}
