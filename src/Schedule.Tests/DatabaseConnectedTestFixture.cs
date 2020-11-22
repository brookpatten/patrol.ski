using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;


using DbUp;
using DbUp.SqlServer;
using Microsoft.Extensions.Configuration;

using NUnit.Framework;
using Schedule.Api.Models;
using Schedule.Api.Services;
using Schedule.Configuration;
using Dapper;
using Schedule.Api.Repositories;

namespace Schedule.Tests
{
    public abstract class DatabaseConnectedTestFixture
    {
        private string _connectionString;
        protected SqlConnection _connection;
        private string _databaseName;
        private SqlConnectionStringBuilder _connectionStringBuilder;
        private const string _dbOpsDb = "master";

        [OneTimeSetUp]
        public void BaseOneTimeSetUp()
        {
            //find the config for the app
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Schedule.Api");
            var builder = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.Local.json", false, true);
            IConfiguration config = builder.Build();
            var serviceConfiguration = config.Get<ScheduleConfiguration>();

            //reformat the existing connection string to connect to the master db (not strictly necassary, but mast always exists, the app db doesn't)
            _connectionStringBuilder = new SqlConnectionStringBuilder(serviceConfiguration.Database.ConnectionString);
        }

        [SetUp]
        public void BaseSetUp()
        {
            //create a connection string to a database we know exists, but is not the db we intend to create/drop
            _connectionStringBuilder.InitialCatalog = _dbOpsDb;
            _connectionString = _connectionStringBuilder.ToString();

            //connect to master and create our test database
            _databaseName = $"{NUnit.Framework.TestContext.CurrentContext.Test.ClassName.Substring(NUnit.Framework.TestContext.CurrentContext.Test.ClassName.LastIndexOf(".") + 1)}-{NUnit.Framework.TestContext.CurrentContext.Test.MethodName}";
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _connection.Execute($"create database [{_databaseName}]");
            _connection.Close();
            _connection.Dispose();

            //reformat the connection sring to point to the test database
            _connectionStringBuilder.InitialCatalog = _databaseName;
            _connectionString = _connectionStringBuilder.ToString();

            //run migrations on the test database
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(_connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetAssembly(typeof(Schedule.Persistence.Migrations.Program)))
                .LogToConsole()
                .Build();
            var result = upgradeEngine.PerformUpgrade();
            if (result.Error != null)
            {
                throw result.Error;
            }

            //initialize the test connection to use the test database
            _connection = new SqlConnection(_connectionString);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _connection.Close();
            _connection.Dispose();

            _connectionStringBuilder.InitialCatalog = _dbOpsDb;
            _connectionString = _connectionStringBuilder.ToString();
            _connection = new SqlConnection(_connectionString);
            _connection.Execute($"alter database [{_databaseName}] set single_user with rollback immediate");
            _connection.Execute($"drop database [{_databaseName}]");
            _connection.Close();
            _connection.Dispose();

            _databaseName = "";
            _connectionString = "";
            _connection = null;
        }
    }
}
