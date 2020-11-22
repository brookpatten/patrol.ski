using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

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

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task TestSqliteConnection()
        {
            using (var connection = new SqliteConnection("" +
                new SqliteConnectionStringBuilder
                {
                    DataSource = "schedule.db"
                }))
            {
                await connection.OpenAsync();

                var existing = await connection.QuerySingleOrDefaultAsync<string>("SELECT name FROM sqlite_master WHERE type='table' AND name='schedules';");
                if(string.IsNullOrEmpty(existing))
                {
                    await connection.ExecuteAsync("CREATE TABLE schedules(id INTEGER PRIMARY KEY ASC, name text);");
                }

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    await connection.InsertAsync(new Schedule() { Name = "extension insert" });

                    await transaction.CommitAsync();
                }
            }
            Ok();
        }
    }
}
