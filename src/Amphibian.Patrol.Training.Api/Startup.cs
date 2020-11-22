using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Security.Claims;
using System.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using FluentValidation.AspNetCore;
using FluentValidation;
using Swashbuckle;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.ReDoc;
using Swashbuckle.AspNetCore.Swagger;
using Serilog;
using Serilog.AspNetCore;

using Amphibian.Patrol.Training.Configuration;
using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Services;
using Amphibian.Patrol.Training.Api.Controllers;
using Amphibian.Patrol.Training.Api.Validations;
using Amphibian.Patrol.Training.Api.Models;

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
            var serviceConfiguration = new PatrolTrainingApiConfiguration();
            Configuration.Bind(serviceConfiguration);

            //pull secure things from env vars if we're not in dev
            //TODO: we can probably just do this with reflection, but this is faster
            SetFromEnvVarIfAvailable<PatrolTrainingApiConfiguration>(serviceConfiguration, (c, s) => c.Email.SendGridApiKey = s, "Email.SendGridApiKey");
            SetFromEnvVarIfAvailable<PatrolTrainingApiConfiguration>(serviceConfiguration, (c, s) => c.Database.ConnectionString = s, "Database.ConnectionString");

            services.AddSingleton(serviceConfiguration);

            services.AddControllers()
                .AddFluentValidation();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
                c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme="bearer",
                    BearerFormat="JWT",
                    In = ParameterLocation.Header,

                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            //Type = SecuritySchemeType.Http,
                            //Flows = new OpenApiOAuthFlows()
                            //{
                            //    ClientCredentials = new OpenApiOAuthFlow(){
                            //        AuthorizationUrl = new Uri("/user/authenticate", UriKind.Relative),
                            //        TokenUrl = new Uri("/user/authenticate", UriKind.Relative),
                            //        Scopes = new Dictionary<string, string>
                            //        {
                            //            { "readAccess", "Access read operations" },
                            //            { "writeAccess", "Access write operations" }
                            //        },
                                    
                            //    }
                            //},
                            //Scheme = "Bearer",
                            //In = ParameterLocation.Header,
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Authorization"
                            }
                        }, new List<string>()
                    }
                });
            });

            services.AddAuthentication("BasicOrTokenAuthentication")
                .AddScheme<AuthenticationSchemeOptions, Infrastructure.AuthenticationHandler>("BasicOrTokenAuthentication", null);

            
            //db
            services.AddScoped<IDbConnection,SqlConnection>(sp=>
            {
                return new SqlConnection(serviceConfiguration.Database.ConnectionString);
            });
            //persistence
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IPatrolRepository, PatrolRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();

            //validations
            services.AddScoped<IValidator<AuthenticationController.RegistrationRequest>, RegistrationValidator>();

            //security
            //services.AddTransient<ClaimsPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

            //services
            services.AddScoped<Services.IAuthenticationService, Services.AuthenticationService>();
            services.AddScoped<Services.IPasswordService, Services.PasswordService>(sp=>new Services.PasswordService(5,32));
            services.AddScoped<EmailService, EmailService>(provider => new EmailService(serviceConfiguration.Email.SendGridApiKey, serviceConfiguration.Email.SendAllEmailsTo,serviceConfiguration.Email.FromName,serviceConfiguration.Email.FromEmail,serviceConfiguration.App.RootUrl));
        }

        private void SetFromEnvVarIfAvailable<T>(T config, Action<T,string> set,string name)
        {
            string value = System.Environment.GetEnvironmentVariable(name);

            if(!string.IsNullOrWhiteSpace(value))
            {
                set(config, value);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
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
            app.UseReDoc(c =>
            {
                c.SpecUrl = "/swagger/v1/swagger.json";
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
