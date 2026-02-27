namespace EcommerceDDD.Core.Infrastructure.OpenTelemetry;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddOpenTelemetryObservability(
        this IServiceCollection services, string serviceName)
    {
        var serviceVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName, serviceVersion: serviceVersion))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddSource(ActivitySources.KafkaConsumer)
                .AddSource(ActivitySources.OutboxWrite)
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter())
            .WithLogging(
                logging => logging.AddOtlpExporter(),
                options =>
                {
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                });

        return services;
    }
}
