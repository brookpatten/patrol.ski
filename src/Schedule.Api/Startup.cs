using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

using Schedule.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Schedule.Api.Repositories;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Schedule.Api
{
    public class Startup
    {

        

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceConfiguration = new ScheduleConfiguration();
            Configuration.Bind(serviceConfiguration);
            services.AddSingleton(serviceConfiguration);

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddAuthentication()
                .AddCookie("PatrolTraining");

            services.AddScoped<IDbConnection,SqlConnection>(sp=>
            {
                return new SqlConnection(serviceConfiguration.Database.ConnectionString);
            });
            services.AddScoped<UserRepository,UserRepository>();
            services.AddScoped<Schedule.Api.Services.AuthenticationService, Schedule.Api.Services.AuthenticationService>(sp=>new Schedule.Api.Services.AuthenticationService(5));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseHttpsRedirection();
            app.UseDefaultFiles(new DefaultFilesOptions() { 
                DefaultFileNames = new List<string>() { "index.html" },
                RequestPath=""
            });
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "../Schedule.Web/dist")),
                    RequestPath = ""
                });
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static")),
                    RequestPath = ""
                });
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
