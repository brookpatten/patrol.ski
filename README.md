# Patrol.Ski ![Test, Build Docker Images, Update Dev](https://github.com/brookpatten/patrol.ski/workflows/Test,%20Build%20Docker%20Images,%20Update%20Dev/badge.svg)

## An App for Ski Patrol

This application consists of a .Net Core 3.1 Web Api application and a static web UI built with node and vue.  The package built from this repository is a docker image, but in theory the application will run fine anywhere capable of hosting a .net core web application and a Sql Server database.

Database schema is managed using DbUp and the Amphibian.Patrol.Ski.Persistence.Migrations exe.  Configuration for migrations is pulled from the API project.  The "environment" is inferred from the "ASPNETCORE_ENVIRONMENT" environment variable, or assumed to be "Local" if default.

Tests are included for the Api Project.  Integration tests have the NUnit category "Persistence" and require a valid database connection string.  The database credentials should have the ability to create and drop databases in order to properly run integration tests.

The web UI is built with CoreUI which combines many front end libraries using node.


To Run the project
 - Create a sql server database, user, and password.  Grant the user dbo.
 - Copy the file appsettings.development.json to appsetting.local.json
 - Update Database.ConnectionString in appsettings.local.json to point to your database
 - Run Amphibian.Patrol.Persistence.Migrations to create the initial database schema
 - Run the Amphibian.Patrol.Api application (this hosts the back end)
 - Navigate to the Amphibian.Patrol.Web project, run npm install, npm run serve to run the front-end

To Run tests
 - Update appsettings.local.json Test.ConnectionString to a valid sql connection string
 - Run Tests in either visual studio or `dotnet test`

If running the docker image, you may (should) set the following environment variables
 - Database__ConnectionString
 - Email__SendGridApiKey
 - Authentication__Facebook__AppId
 - Authentication__Google__ClientId
 - Authentication__Microsoft__ClientId
 - Authentication__Microsoft__TenantId
 - Authentication__Microsoft__GraphBaseEndpoint
