using System;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;

namespace Amphibian.Patrol.Training.Configuration
{
    public class PatrolTrainingApiConfiguration
    {
        public DatabaseConfiguration Database { get; set; }
        public TestConfiguration Test { get; set; }
        public EmailConfiguration Email{get;set;}
        public AppConfiguration App { get; set; }

        public PatrolTrainingApiConfiguration()
        {
            Database = new DatabaseConfiguration();
            Test = new TestConfiguration();
            Email = new EmailConfiguration();
            App = new AppConfiguration();
        }

        public static PatrolTrainingApiConfiguration LoadFromJsonConfig(string configBasePath=null,string environmentName=null)
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

            var serviceConfiguration = config.Get<PatrolTrainingApiConfiguration>();

            return serviceConfiguration;
        }
    }

    public class AppConfiguration
    {
        public string RootUrl { get; set; }
    }

    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public bool MigrateSchema { get; set; }
        public bool MigrateInitialData { get; set; }
        public bool MigrateTestData { get; set; }
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

    public class EmailConfiguration
    {
        public string SendGridApiKey { get; set; }
        public string SendAllEmailsTo { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
    }
}
