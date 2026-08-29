namespace EcommerceDDD.Core.Infrastructure.Marten;

public static class MartenConfigExtension
{
    public static void AddMarten(this IServiceCollection services, 
        ConfigurationManager configuration,
        Action<StoreOptions>? configureOptions = null)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var martenConfig = configuration.GetSection("EventStore")
            .Get<MartenSettings>();

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException("EventStore connection string is missing");

        if (string.IsNullOrEmpty(martenConfig?.WriteSchema))
            throw new ArgumentNullException("EventStore writeSchema is missing");

        var martenConfiguration = services.AddMarten(options =>
        {
            options.Connection(connectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
			options.Events.DatabaseSchemaName = martenConfig.WriteSchema;

            options.UseNewtonsoftForSerialization(
                nonPublicMembersStorage: NonPublicMembersStorage.All);

            if (!string.IsNullOrEmpty(martenConfig.ReadSchema))
                options.DatabaseSchemaName = martenConfig.ReadSchema;

            // Custom store options
            configureOptions?.Invoke(options);
        }).UseLightweightSessions();

        // Wolverine's inbox/outbox tables live in the same database, created by Marten's
        // schema management. MartenRepository takes IMartenOutbox and this registration is what supplies it.
        martenConfiguration.IntegrateWithWolverine();

		// Wrapper for IQuerySession 
		services.AddScoped<IQuerySessionWrapper, QuerySessionWrapper>();
	}
}
