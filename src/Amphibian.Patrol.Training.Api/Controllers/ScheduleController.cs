using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }

    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(ILogger<ScheduleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task Test()
        {
        }
    }
}
