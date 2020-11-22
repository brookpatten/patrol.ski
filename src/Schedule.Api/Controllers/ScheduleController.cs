using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;

namespace Schedule.Api.Controllers
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
        public async Task TestMsSqlConnection()
        {


            using (var connection = new SqlConnection("Server=tcp:trainingschedule.database.windows.net,1433;Initial Catalog=trainingschedule;Persist Security Info=False;User ID=trainingschedule;Password=Catalina25;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                await connection.OpenAsync();

                await connection.CloseAsync();

                Ok();
            }

        }

    }
}
