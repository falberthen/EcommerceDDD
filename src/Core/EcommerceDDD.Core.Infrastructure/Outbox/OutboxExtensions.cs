using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Core.Infrastructure.Workers;
using EcommerceDDD.Core.Infrastructure.Outbox.Persistence;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Core.Infrastructure.Outbox;

public static class OutboxExtensions
{
    public static void AddOutboxSetup(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        string connString = configuration.GetConnectionString("OutboxConnection");

        services.AddDbContext<OutboxDbContext>(options =>
        {
            options.UseNpgsql(connString,
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
        });

        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddTransient<IOutboxMessageService, OutboxMessageService>();

        services.AddHostedService<OutboxMessageProcessor>();
    }
}