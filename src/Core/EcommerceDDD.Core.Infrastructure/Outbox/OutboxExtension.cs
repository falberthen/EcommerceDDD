namespace EcommerceDDD.Core.Infrastructure.Outbox;

public static class OutboxExtension
{
    public static IServiceCollection ConfigureDebezium(this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        // ---- Settings
        var debeziumSettings = configuration.GetSection("DebeziumSettings");

        if(debeziumSettings is not null)
        {
            services.Configure<DebeziumSettings>(debeziumSettings);
            services.TryAddSingleton<IDebeziumConnectorSetup, DebeziumConnectorSetup>();
            services.AddHostedService<DebeziumBackgroundWorker>();
        }

        return services;
    }
}