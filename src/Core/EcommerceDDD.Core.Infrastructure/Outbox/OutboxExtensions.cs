namespace EcommerceDDD.Core.Infrastructure.Outbox;

public static class OutboxExtensions
{
    public static void ConfigureDebezium(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        // ---- Settings
        var debeziumSettings = configuration.GetSection("DebeziumSettings");
        services.Configure<DebeziumSettings>(debeziumSettings);

        services.TryAddSingleton<IDebeziumConnectorSetup, DebeziumConnectorSetup>();
        services.AddHostedService(serviceProvider =>
        {
            var consumer = serviceProvider.GetRequiredService<IDebeziumConnectorSetup>();
            return new BackgroundWorker(consumer.StartConfiguringAsync);
        });
    }
}