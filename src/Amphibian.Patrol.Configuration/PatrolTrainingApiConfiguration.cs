using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;
using System.Net.Http;

namespace Amphibian.Patrol.Configuration
{
    public class PatrolTrainingApiConfiguration
    {
        public DatabaseConfiguration Database { get; set; }
        public TestConfiguration Test { get; set; }
        public EmailConfiguration Email { get; set; }
        public AppConfiguration App { get; set; }
        public AuthenticationConfiguration Authentication { get; set; }
        public string Version { get; set; }

        public PatrolTrainingApiConfiguration()
        {
            Database = new DatabaseConfiguration();
            Test = new TestConfiguration();
            Email = new EmailConfiguration();
            App = new AppConfiguration();
        }

        public static string Environment
        {
            get
            {
                var environmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (string.IsNullOrEmpty(environmentName))
                {
                    environmentName = "Local";
                }
                return environmentName;
            }
        }

        public static (IConfiguration,PatrolTrainingApiConfiguration) LoadFromJsonConfig(IConfigurationBuilder builder=null, params string[] basePaths)
        {
            var checkPaths = basePaths.ToList();
            var assemblyDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!checkPaths.Any(x=>x== assemblyDirectory))
            {
                checkPaths.Add(assemblyDirectory);
            }

            string configBasePath = null;
            foreach (var path in checkPaths)
            {
                if (File.Exists(Path.Combine(path, "appsettings.json")))
                {
                    configBasePath = path;
                    
                    Console.WriteLine("Configuration: " + path);
                    //logger.LogInformation($"Found Configuration in {configBasePath}");
                    break;
                }
            }

            if (!File.Exists(Path.Combine(configBasePath, "appsettings.json")))
            {
                //logger.LogCritical("Failed to find configuration in {@basePaths}",basePaths);
                throw new FileNotFoundException("Failed to find appsettings.json");
            }

            var environmentName = Environment;
            Console.WriteLine("Environment: "+environmentName);
            var machineName = System.Environment.MachineName;
            Console.WriteLine("Machine: " + machineName);

            //logger.LogInformation($"Loading Configuration For Environment {environmentName} Machine {machineName}");

            if(builder==null)
            {
                builder = new ConfigurationBuilder();
            }

            //builder.Sources.Clear();

            builder = builder
                .SetBasePath(configBasePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: false)
                .AddJsonFile($"appsettings.{machineName}.machine.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.{machineName}.machine.json", optional: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            
            var serviceConfiguration = config.Get<PatrolTrainingApiConfiguration>();

            var connStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(serviceConfiguration.Database.ConnectionString);
            Console.WriteLine("Database: " + connStringBuilder.DataSource + " "+ connStringBuilder.InitialCatalog);

            return (config,serviceConfiguration);
        }
    }

    public class AppConfiguration
    {
        public string RootUrl { get; set; }
        public string JwtKey { get; set; }
        public int? DemoPatrolId { get; set; }
        public string [] SystemAdministrators { get; set; }
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
        public string ConnectionString { get; set; }

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
        public string ProfileRoute { get; set; }
    }

    public class AuthenticationConfiguration
    {
        public GoogleAuthenticationConfiguration Google { get; set; }
        public FacebookAuthenticationConfiguration Facebook { get; set; }
        public MicrosoftAuthenticationConfiguration Microsoft { get; set; }
    }

    public class GoogleAuthenticationConfiguration
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }

    public class FacebookAuthenticationConfiguration
    {
        public string AppId { get; set; }
    }

    public class MicrosoftAuthenticationConfiguration
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string GraphBaseEndpoint { get; set; }
    }
}
