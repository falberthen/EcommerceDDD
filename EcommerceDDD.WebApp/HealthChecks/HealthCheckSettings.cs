using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceDDD.WebApp.HealthChecks
{
    public class HealthCheckSettings
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Uri { get; set; }
        [Required]
        public string HealthCheckDatabaseConnectionString { get; set; }
        [Required]
        public int EvaluationTimeinSeconds { get; set; }
        [Required]
        public int MinimumSecondsBetweenFailureNotifications { get; set; }

        public static HealthCheckSettings FromConfiguration(IConfiguration configuration)
        {
            if (configuration["HealthCheck:Name"] != null)
            {
                var result = new HealthCheckSettings();
                configuration.Bind("HealthCheck", result);
                return result;
            }
            return null;
        }
    }
}
