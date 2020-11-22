using System;
using System.Reflection;
using System.IO;

using DbUp;
using DbUp.SqlServer;

using Amphibian.Patrol.Training.Configuration;

namespace Amphibian.Patrol.Training.Persistence.Migrations
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configuration = PatrolTrainingApiConfiguration.LoadFromJsonConfig(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Amphibian.Patrol.Training.Api"));
            var result = MigrationRunner.RunMigrations(configuration.Database.ConnectionString,true,false);

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
