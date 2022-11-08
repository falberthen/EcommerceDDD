using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Core.Infrastructure.Workers;
using EcommerceDDD.Core.Infrastructure.Outbox.Workers;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;
using EcommerceDDD.Core.Infrastructure.Outbox.Persistence;

namespace EcommerceDDD.Core.Infrastructure.Outbox;

public static class OutboxExtensions
{

    public static IHost EnsureDatabaseCreated(this IHost host, WebApplicationBuilder builder)
    {
        using (var scope = host.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<OutboxDbContext>().Database.EnsureCreated();
        }

        return host;
    }

    public static void AddOutboxDebeziumConnector(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddOutboxSetup(services, builder);

        services.AddHostedService(serviceProvider =>
        {
            var consumer = serviceProvider.GetRequiredService<IDebeziumConnectorSetup>();
            return new BackgroundWorker(consumer.StartConfiguringAsync);
        });
    }

    public static void AddOutboxPollingService(this IServiceCollection services, WebApplicationBuilder builder)
    {
        AddOutboxSetup(services, builder);

        services.AddHostedService(serviceProvider =>
        {
            var consumer = serviceProvider.GetRequiredService<IOutboxMessageProcessor>();
            return new BackgroundWorker(consumer.StartProcessingAsync);
        });
    }

    private static void AddOutboxSetup(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        // ---- Settings
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var outboxSettings = builder.Configuration.GetSection("OutboxSettings");
        builder.Services.Configure<OutboxSettings>(outboxSettings);

        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddTransient<IOutboxMessageService, OutboxMessageService>();
        services.AddTransient<IOutboxMessageProcessor, OutboxMessageProcessor>();
        services.AddTransient<IDebeziumConnectorSetup, DebeziumConnectorSetup>();
        
        services.AddDbContext<OutboxDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
        });
    }
}