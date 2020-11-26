using DbUp.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using DbUp;

using Amphibian.Patrol.Configuration;

namespace Amphibian.Patrol.Persistence.Migrations
{
    public static class MigrationRunner
    {
        public static DatabaseUpgradeResult RunMigrations(string connectionString, bool includeInitialData, bool includeTestData)
        {
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), scriptName => {
                    return (includeInitialData || !scriptName.Contains("-InitialData"))
                        && (includeTestData || !scriptName.Contains("-TestData"));
                })
                .WithTransaction()
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();

            return result;
        }

        public static void RunMigrationsThrowOnException(string connectionString, bool includeInitialData, bool includeTestData)
        {
            var result = RunMigrations(connectionString, includeInitialData, includeTestData);

            if(result.Error!=null)
            {
                throw result.Error;
            }
        }
    }
}
