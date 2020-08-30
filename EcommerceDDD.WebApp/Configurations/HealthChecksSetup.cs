using EcommerceDDD.WebApp.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Configuration;

namespace EcommerceDDD.WebApp.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public static class HealthChecksSetup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddHealthChecksSetup(this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = HealthCheckSettings.FromConfiguration(configuration);

            if (services == null ) 
                throw new ArgumentNullException(nameof(services));

            string cnString = configuration.GetConnectionString("DefaultConnection");

            services.AddHealthChecksUI(setupSettings: options =>
            {
                var settings = HealthCheckSettings.FromConfiguration(configuration);
                if (settings != null)
                {
                    options.AddHealthCheckEndpoint(
                        settings.Name,
                        settings.Uri
                    );
                    //options.SetHealthCheckDatabaseConnectionString(settings.HealthCheckDatabaseConnectionString);
                    options.SetEvaluationTimeInSeconds(settings.EvaluationTimeinSeconds);
                    options.SetMinimumSecondsBetweenFailureNotifications(settings.MinimumSecondsBetweenFailureNotifications);
                }
            });

            services.AddHealthChecks()
            .AddCheck("EcommerceDDD-check", 
            new SqlConnectionHealthCheck(cnString), 
            HealthStatus.Unhealthy, new string[] { "EcommerceDDD" });
        }
    }
}
