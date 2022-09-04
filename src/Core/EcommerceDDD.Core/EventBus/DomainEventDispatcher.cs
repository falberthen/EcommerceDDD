using MediatR;
using EcommerceDDD.Core.Domain;
using Microsoft.Extensions.Logging;

namespace EcommerceDDD.Core.EventBus;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger)
    {
        _mediator = mediator;
        _logger = logger;

        if (_mediator == null)
            throw new ArgumentNullException(nameof(_mediator));
    }

    public async Task DispatchAsync(IDomainEvent @event)
    {
        var domainEventNotification = CreateDomainEventNotification(@event);

        _logger.LogInformation("Publishing event...", domainEventNotification);
        await _mediator.Publish(domainEventNotification);
    }

    private INotification CreateDomainEventNotification(IDomainEvent @event)
    {
        var notificationType = typeof(DomainEventNotification<>)
            .MakeGenericType(@event.GetType());

        return (INotification)Activator.CreateInstance(notificationType, @event);
    }
}
