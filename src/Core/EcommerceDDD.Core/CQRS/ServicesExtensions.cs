using MediatR;
using EcommerceDDD.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.CQRS;

public static class ServicesExtensions
{
    public static void ConfigureCQRS(this IServiceCollection services)
    {
        services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();
    }
}
