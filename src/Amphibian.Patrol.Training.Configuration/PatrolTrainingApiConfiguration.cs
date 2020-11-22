using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Amphibian.Patrol.Training.Configuration
{
    public class PatrolTrainingApiConfiguration
    {
        public AzureConfiguration Azure { get; set; }

        public DatabaseConfiguration Database { get; set; }
        public TestConfiguration Test { get; set; }
        public EmailConfiguration Email{get;set;}
        public AppConfiguration App { get; set; }

        public PatrolTrainingApiConfiguration()
        {
            Azure = new AzureConfiguration();
            Database = new DatabaseConfiguration();
            Test = new TestConfiguration();
            Email = new EmailConfiguration();
            App = new AppConfiguration();
        }

        public static (IConfiguration,PatrolTrainingApiConfiguration) LoadFromJsonConfig(IConfigurationBuilder builder=null, params string[] basePaths)
        {
            var checkPaths = basePaths.ToList();
            if (!checkPaths.Any(x=>x==""))
            {
                Console.WriteLine("Adding Default Path");
                checkPaths.Add("");
            }
            var assemblyDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!checkPaths.Any(x=>x== assemblyDirectory))
            {
                Console.WriteLine("Adding " + assemblyDirectory);
                checkPaths.Add(assemblyDirectory);
            }

            string configBasePath = null;
            foreach (var path in basePaths)
            {
                Console.WriteLine("Checking " + path);
                if (File.Exists(Path.Combine(path, "appsettings.json")))
                {
                    configBasePath = path;
                    Console.WriteLine("Found appsettings.json in " + path);
                    //logger.LogInformation($"Found Configuration in {configBasePath}");
                    break;
                }
            }

            if (string.IsNullOrEmpty(configBasePath))
            {
                //logger.LogCritical("Failed to find configuration in {@basePaths}",basePaths);
                throw new FileNotFoundException("Failed to find appsettings.json");
            }

            //this is a little backward, but we always read the env from the environment variable so that we can figure out which config to load
            //the config could potentially contain an environment as well
            //this name "ASPNETCORE_ENVIRONMENT" is special for asp.net core and triggers other behaviors in asp.net core
            var environmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = "Local";
            }
            Console.WriteLine("Environment: "+environmentName);
            var machineName = System.Environment.MachineName;
            Console.WriteLine("Machine: " + machineName);

            //logger.LogInformation($"Loading Configuration For Environment {environmentName} Machine {machineName}");

            if(builder==null)
            {
                builder = new ConfigurationBuilder();
            }

            builder = new ConfigurationBuilder()
                .SetBasePath(configBasePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: false)
                .AddJsonFile($"appsettings.{machineName}.machine.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.{machineName}.machine.json", optional: true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            var serviceConfiguration = config.Get<PatrolTrainingApiConfiguration>();

            //if config specifies an azure secret url, we need to load those secrets into config too
            if (!string.IsNullOrEmpty(serviceConfiguration.Azure.KeyVaultUrl))
            {
                Console.Write("Configuring from Key Vault " + serviceConfiguration.Azure.KeyVaultUrl);
                //logger.LogInformation($"Loading Additional Secret Configuration from Azure Key Vault {serviceConfiguration.SecretAzureKeyVaultUrl}");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
                builder.AddAzureKeyVault(serviceConfiguration.Azure.KeyVaultUrl, keyVaultClient, new DefaultKeyVaultSecretManager());

                //do it again, this time with azure keyvault
                config = builder.Build();
                serviceConfiguration = config.Get<PatrolTrainingApiConfiguration>();
            }

            return (config,serviceConfiguration);
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

    public class AzureConfiguration
    {
        public string KeyVaultUrl { get; set; }
    }
}
