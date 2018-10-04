﻿namespace Mega.Web.Api
{
    using System;

    using Mega.Services;
    using Mega.Web.Api.Infrastructure.IoC;
    using Mega.Web.Api.Middleware;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using StructureMap;

    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ApplicationLogging.LoggerFactory.AddEventLog(LogLevel.Debug);
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Crawler API", Version = "v1" });
                    c.IncludeXmlComments("Mega.Web.Api.xml");
                });

            var container = new Container();

            container.Configure(config =>
                {
                    config.AddRegistry(new DataInstaller(this.Configuration));
                    config.Populate(services);
                });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddEventLog(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<UnhandledExceptionMiddleware>();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/info/swagger.json", "Crawler API");
                    c.RoutePrefix = "help";
                });
            app.UseMvc();
        }
    }
}
