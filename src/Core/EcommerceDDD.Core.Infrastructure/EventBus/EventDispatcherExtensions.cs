using EcommerceDDD.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.Infrastructure.EventBus;

public static class EventDispatcherExtensions
{
    public static void AddEventDispatcher(this IServiceCollection services)
    {
        services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        services.AddTransient<IEventDispatcher, EventDispatcher>();
    }
}
