namespace EcommerceDDD.Core.Infrastructure;

public static class CoreInfrastructureExtensions
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddMemoryCache()
            .AddHttpContextAccessor()
            // CQRS
            .AddScoped<ICommandBus, CommandBus>()
            .AddScoped<IQueryBus, QueryBus>()
            // Identity
            .AddJwtAuthentication(configuration)
            // Http integration
            .ConfigureIntegrationHttpService(configuration)
            // Swagger extensions
            .AddSwagger(configuration)
            // Testing
            .AddScoped<IEventStoreRepository<DummyAggregateRoot>,
                DummyEventStoreRepository<DummyAggregateRoot>>()
            // EventBus
            .TryAddSingleton<IEventPublisher, EventPublisher>();

        return services;
    }
}
