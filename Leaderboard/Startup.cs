using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leaderboard.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using Swashbuckle.AspNetCore.Swagger;

namespace leaderboard
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDistributedRedisCache(o => {
                o.Configuration = "localhost";
                o.InstanceName = "SampleInstance";
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Leaderboard API", Version = "v1" });
            });
            services.Add(new ServiceDescriptor(
                typeof(IRedisClientsManager),
                c => new BasicRedisClientManager(Configuration.GetConnectionString("redis")),
                ServiceLifetime.Singleton
            ));
            services.Add(new ServiceDescriptor(
                typeof(ScoreRepository),
                c => new ScoreRepository(c.GetService<IRedisClientsManager>()),
                ServiceLifetime.Singleton
            ));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("Leaderboard/appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Leaderboard/appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leaderboard API V1");
            });
        }

    }
}
