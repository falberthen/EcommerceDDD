using MediatR;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Application.FinalizingShipment;

public class PackageDeliveredHandler : INotificationHandler<DomainNotification<PackageDelivered>>
{
    private readonly IEventProducer _eventProducer;

    public PackageDeliveredHandler(IEventProducer eventProducer)
    {
        _eventProducer = eventProducer;
    }

    public async Task Handle(DomainNotification<PackageDelivered> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;

        // Notifying Order Saga
        await _eventProducer.PublishAsync(
            new OrderDelivered(
                @event.ShipmentId,
                @event.OrderId),
                cancellationToken
            );
    }
}