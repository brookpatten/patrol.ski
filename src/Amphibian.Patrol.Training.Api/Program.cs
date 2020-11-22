using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Amphibian.Patrol.Training.Configuration;

namespace Amphibian.Patrol.Training.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var host = builder.Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var configurations = PatrolTrainingApiConfiguration.LoadFromJsonConfig(config);

            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseKestrel()
                    .UseStartup<Startup>();
            })
            .UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext();
            });

            return builder;
        }
    }
}
