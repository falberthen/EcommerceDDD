namespace EcommerceDDD.Core.Infrastructure;

public static class CoreInfrastructureExtensions
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        
        // EventBus
        services.TryAddSingleton<IEventDispatcher, EventDispatcher>();

        // CQRS
        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryBus, QueryBus>();

        // Identity
        services.AddJwtAuthentication(configuration);

        // Http integration
        services.ConfigureIntegrationHttpService(configuration);

        // Swagger extensions
        services.AddSwagger(configuration);

        // Testing
        services.AddScoped<IEventStoreRepository<DummyAggregateRoot>, 
            DummyEventStoreRepository<DummyAggregateRoot>>();

        return services;
    }
}
