using Marten;
using Weasel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.Infrastructure.Marten;

public static class MartenConfigExtensions
{
    public static void AddMarten(this IServiceCollection services, 
        ConfigurationManager configuration,
        Action<StoreOptions>? configureOptions = null)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var martenConfig = configuration.GetSection("EventStore")
            .Get<MartenSettings>();

        if (string.IsNullOrEmpty(martenConfig.ConnectionString))
            throw new ArgumentNullException("EventStore connection string is missing");

        if (string.IsNullOrEmpty(martenConfig.WriteSchema))
            throw new ArgumentNullException("EventStore writeSchema is missing");

        services.AddMarten(options =>
        {
            options.Connection(martenConfig.ConnectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;            
            options.UseDefaultSerialization(nonPublicMembersStorage: NonPublicMembersStorage.All);
            options.Events.DatabaseSchemaName = martenConfig.WriteSchema;

            if (!string.IsNullOrEmpty(martenConfig.ReadSchema))
                options.DatabaseSchemaName = martenConfig.ReadSchema;

            // Custom store options
            configureOptions?.Invoke(options);
        });
    }
}
