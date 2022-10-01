using MediatR;
using EcommerceDDD.Core.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.EventBus;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceScopeFactory serviceScopeFactory, ILogger<DomainEventDispatcher> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent @event)
    {
        var domainNotification = CreateDomainNotification(@event);

        _logger.LogInformation("Publishing domain notification...", domainNotification);

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var mediator = scopedServices.GetRequiredService<IMediator>();
            await mediator.Publish(domainNotification);
        }
    }

    private INotification CreateDomainNotification(IDomainEvent @event)
    {
        var notificationType = typeof(DomainNotification<>)
            .MakeGenericType(@event.GetType());

        return (INotification)Activator.CreateInstance(notificationType, @event);
    }
}
