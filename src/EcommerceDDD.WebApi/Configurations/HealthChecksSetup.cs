using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Configuration;

namespace EcommerceDDD.WebApi.Configurations
{
    /// <summary>
    /// HealthCheck Setup
    /// </summary>
    public static class HealthChecksSetup
    {
        public static void AddHealthChecksSetup(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null ) 
                throw new ArgumentNullException(nameof(services));

            string cnString = configuration.GetConnectionString("DefaultConnection");

            services.AddHealthChecks()
            .AddSqlServer(cnString, null, "SQL Server");

            services.AddHealthChecksUI().AddInMemoryStorage();
        }
    }
}
