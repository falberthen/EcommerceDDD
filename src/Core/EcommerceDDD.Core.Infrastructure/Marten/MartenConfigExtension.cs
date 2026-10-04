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
        
		// Filtering out non-essential sql traces to keep the dashboard cleaner.
        var dataSource = new NpgsqlDataSourceBuilder(connectionString)
            .ConfigureTracing(tracing => tracing
                .ConfigureCommandFilter(_ => Activity.Current is not null)
                .ConfigureBatchFilter(_ => Activity.Current is not null)
                .EnablePhysicalOpenTracing(false))
            .Build();

        var martenConfiguration = services.AddMarten(options =>
        {
            options.Connection(dataSource);
            options.AutoCreateSchemaObjects = AutoCreate.All;
			options.Events.DatabaseSchemaName = martenConfig.WriteSchema;

            options.UseNewtonsoftForSerialization(
                nonPublicMembersStorage: NonPublicMembersStorage.All);

            if (!string.IsNullOrEmpty(martenConfig.ReadSchema))
                options.DatabaseSchemaName = martenConfig.ReadSchema;

            // Custom store options
            configureOptions?.Invoke(options);
        }).UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();

        // Wolverine's inbox/outbox tables live in the same database, created by Marten's
        // schema management. MartenRepository takes IMartenOutbox and this registration is what supplies it.
        martenConfiguration.IntegrateWithWolverine();

		// Wrapper for IQuerySession 
		services.AddScoped<IQuerySessionWrapper, QuerySessionWrapper>();
	}
}
