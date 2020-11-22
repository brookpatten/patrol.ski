using System;
using System.Reflection;
using System.IO;

using DbUp;
using DbUp.SqlServer;

using Amphibian.Patrol.Training.Configuration;
using Microsoft.Extensions.Configuration;

namespace Amphibian.Patrol.Training.Persistence.Migrations
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = PatrolTrainingApiConfiguration.LoadFromJsonConfig(new ConfigurationBuilder(),Path.Combine(Directory.GetCurrentDirectory(), "../../../../Amphibian.Patrol.Training.Api"));

            if (configuration.Database.MigrateSchema)
            {
                var result = MigrationRunner.RunMigrations(configuration.Database.ConnectionString, configuration.Database.MigrateInitialData, configuration.Database.MigrateTestData);

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
            else
            {
                Console.WriteLine("Migrating Schema is Disabled in Configuration");
            }
        }

        
    }
}
