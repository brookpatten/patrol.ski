using System;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;

namespace Amphibian.Patrol.Training.Configuration
{
    public class ScheduleConfiguration
    {
        public DatabaseConfiguration Database { get; set; }
        public TestConfiguration Test { get; set; }

        public ScheduleConfiguration()
        {
            Database = new DatabaseConfiguration();
            Test = new TestConfiguration();
        }

        public static ScheduleConfiguration LoadFromJsonConfig(string configBasePath=null,string environmentName=null)
        {
            if(string.IsNullOrEmpty(environmentName))
            {
                environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");
                if(string.IsNullOrEmpty(environmentName))
                {
                    environmentName = "Local";
                }
            }

            if(string.IsNullOrEmpty(configBasePath))
            {
                configBasePath = Directory.GetCurrentDirectory();
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(configBasePath)
                .AddJsonFile("appsettings.json", false, true);

            if (File.Exists(Path.Combine(configBasePath, $"appsettings.{environmentName}.json")))
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json",false,true);
            }

            IConfiguration config = builder.Build();

            var serviceConfiguration = config.Get<ScheduleConfiguration>();

            return serviceConfiguration;
        }
    }

    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
    }

    public class TestConfiguration
    {
        public enum PersistenceTestCompletionAction { DropOnComplete, RenameOnComplete, DropIfSuccessRenameIfFailures, DropIfSuccess }
        public PersistenceTestCompletionAction OnPersistenceTestCompletion { get; set; }

        public TestConfiguration()
        {
            OnPersistenceTestCompletion = PersistenceTestCompletionAction.DropOnComplete;
        }
    }
}
