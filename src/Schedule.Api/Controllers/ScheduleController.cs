using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;

namespace Schedule.Api.Controllers
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(ILogger<ScheduleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task Test()
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
