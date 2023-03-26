using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Persistence;
using Microsoft.Extensions.Configuration;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.CQRS.QueryHandling;
using EcommerceDDD.Core.Infrastructure.CQRS;
using EcommerceDDD.Core.Infrastructure.WebApi;
using EcommerceDDD.Core.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure.Integration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EcommerceDDD.Core.Infrastructure;

public static class CoreInfrastructureExtensions
{
    public static void AddInfrastructureExtension(this IServiceCollection services, IConfiguration configuration)
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
    }
}
