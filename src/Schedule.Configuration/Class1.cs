using System;

namespace Schedule.Configuration
{
    public class ScheduleConfiguration
    {
        public DatabaseConfiguration Database { get; set; }
    }

    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
