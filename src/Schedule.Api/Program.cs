using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Schedule.Api
{
    public class Program
    {
        public static void Main(string[] args)
        { 
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            if(string.IsNullOrEmpty(environment))
            {
                environment = "Local";
            }

            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                });


            if (!string.IsNullOrEmpty(environment))
            {
                var envSpecificConfig = $"appsettings.{environment}.json";
                if (File.Exists(envSpecificConfig))
                {
                    builder = builder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json");
                        config.AddJsonFile(envSpecificConfig);
                    });
                }
            }

            builder = builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

            return builder;
        }
    }
}
