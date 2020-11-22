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

using Amphibian.Patrol.Training.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Amphibian.Patrol.Training.Api.Repositories;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Amphibian.Patrol.Training.Api
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            services.AddAuthentication("BasicOrTokenAuthentication")
                .AddScheme<AuthenticationSchemeOptions, Infrastructure.AuthenticationHandler>("BasicOrTokenAuthentication", null);

            services.AddScoped<IDbConnection,SqlConnection>(sp=>
            {
                return new SqlConnection(serviceConfiguration.Database.ConnectionString);
            });
            services.AddTransient<ClaimsPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IPatrolRepository, PatrolRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<Amphibian.Patrol.Training.Api.Services.IAuthenticationService, Amphibian.Patrol.Training.Api.Services.AuthenticationService>();
            services.AddScoped<Amphibian.Patrol.Training.Api.Services.IPasswordService, Amphibian.Patrol.Training.Api.Services.PasswordService>(sp=>new Amphibian.Patrol.Training.Api.Services.PasswordService(5,32));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseHttpsRedirection();

            string staticFilesPath;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "../Amphibian.Patrol.Training.Web/dist");

            }
            else
            {
                staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "static");
            }

            if (Directory.Exists(staticFilesPath))
            {
                app.UseDefaultFiles(new DefaultFilesOptions()
                {
                    DefaultFileNames = new List<string>() { "index.html" },
                    FileProvider = new PhysicalFileProvider(staticFilesPath)
                });
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(staticFilesPath),
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
