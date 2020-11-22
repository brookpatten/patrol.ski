using System;
using System.Reflection;
using System.IO;

using DbUp;
using DbUp.SqlServer;
using Microsoft.Extensions.Configuration;

using Schedule.Configuration;

namespace Schedule.Persistence.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            string configPath = "";
            if (string.IsNullOrEmpty(environment))
            {
                environment = "Local";
                configPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Schedule.Api");
            }
            else
            {
                //same path as bins?
                configPath = "";
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json", true, true);

            if(File.Exists(Path.Combine(configPath,$"appsettings.{environment}.json")))
            {
                builder = builder.AddJsonFile($"appsettings.{environment}.json");
            }

            IConfiguration config=builder.Build();

            var serviceConfiguration = config.Get<ScheduleConfiguration>();


            var upgradeEngine = DeployChanges.To
                .SqlDatabase(serviceConfiguration.Database.ConnectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
        }

        
    }
}
