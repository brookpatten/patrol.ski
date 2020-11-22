using System;
using System.Reflection;

using DbUp;
using DbUp.SqlServer;

namespace Schedule.Persistence.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            var upgradeEngine = DeployChanges.To
                .SqlDatabase("Server=kessel\\sqlexpress;Initial Catalog=trainingschedule;Trusted_Connection=True;User ID=trainingschedule;Password=trainingschedule;")
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
