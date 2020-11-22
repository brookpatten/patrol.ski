# Patrol.Training

## Assign, Organize, and track Outdoor Emergency Transportation (OET) Training for Ski Patrol

This application consists of a .Net Core 3.1 Web Api application and a static web UI built with node and vue.  Default deployment is for azure, but it can be hosted on any webhost capable of hosting static files and .Net Core 3.1.

Database schema is managed using DbUp and the Amphibian.Patrol.Training.Persistence.Migrations exe.  Configuration for migrations is pulled from the API project.  The "environment" is inferred from the "ENVIRONMENT" environment variable, or assumed to be "Local" if default.

Tests are included for the Api Project.  Integration tests have the NUnit category "Persistence" and require a valid database connection string.  The database credentials should have the ability to create and drop databases in order to properly run integration tests.

The web UI is built with CoreUI which combines many front end libraries using node.
