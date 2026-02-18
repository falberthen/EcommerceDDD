using OpenTelemetry.Logs;

namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddOpenTelemetryObservability(
        this IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter();
            });

        services.AddLogging(logging =>
        {
            logging.AddOpenTelemetry(otel =>
            {
                otel.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName));
                otel.IncludeFormattedMessage = true;
                otel.IncludeScopes = true;
                otel.ParseStateValues = true; // Parse structured state values
                otel.AddOtlpExporter();
            });
        });

        return services;
    }
}
