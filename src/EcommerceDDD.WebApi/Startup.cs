using EcommerceDDD.WebApi.BackgroundServices;
using EcommerceDDD.WebApi.Configurations;
using EcommerceDDD.Infrastructure.IoC;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Application.SignalR;

namespace EcommerceDDD.WebApi
{
    public class Startup
    {
        public IWebHostEnvironment Env { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => 
                o.AddPolicy("CorsPolicy", builder => {
                    builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200");
                }
            ));

            // Setting DBContexts
            services.AddDatabaseSetup(Configuration);

            // ASP.NET Identity Settings & JWT
            services.AddIdentitySetup(Configuration);

            services.AddAuthSetup(Configuration);

            // AutoMapper Settings
            services.AddAutoMapperSetup();

            // WebAPI Config
            services.AddControllers();

            // Adding MediatR
            services.AddMediatR(typeof(Startup));

            // .NET Native DI Abstraction
            services.RegisterServices();

            // Swagger Config
            services.AddSwaggerSetup();

            // Health Checks
            services.AddHealthChecksSetup(Configuration);

            // SignalR support
            services.AddSignalR();

            // Message processing
            var section = this.Configuration.GetSection(nameof(MessageProcessorServiceOptions));
            var messageProcessorTaskOptions = section.Get<MessageProcessorServiceOptions>();
            services.AddSingleton(messageProcessorTaskOptions);
            services.AddHostedService<MessagesProcessorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseSwaggerSetup();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            // HealthCheck middleware
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(config => config.UIPath = "/hc-ui");

            // SignalR
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<OrderStatusHub>("api/orderstatushub");
            });
        }
    }
}
