using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Core.Infrastructure.Workers;
using EcommerceDDD.Core.Infrastructure.Outbox.Workers;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EcommerceDDD.Core.Infrastructure.Outbox;

public static class OutboxExtensions
{
    public static void AddOutboxService(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        // ---- Settings
        var debeziumSettings = configuration.GetSection("DebeziumSettings");
        services.Configure<DebeziumSettings>(debeziumSettings);

        services.AddScoped<IOutboxMessageService, OutboxMessageService>();
        services.TryAddSingleton<IDebeziumConnectorSetup, DebeziumConnectorSetup>();
        services.AddHostedService(serviceProvider =>
        {
            var consumer = serviceProvider.GetRequiredService<IDebeziumConnectorSetup>();
            return new BackgroundWorker(consumer.StartConfiguringAsync);
        });
    }
}