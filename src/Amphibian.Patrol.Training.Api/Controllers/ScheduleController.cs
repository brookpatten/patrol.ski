using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dommel;

using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Amphibian.Patrol.Training.Api.Extensions;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }

    [ApiController]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        
        public ScheduleController(ILogger<ScheduleController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        [Route("[controller]/[action]")]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}
