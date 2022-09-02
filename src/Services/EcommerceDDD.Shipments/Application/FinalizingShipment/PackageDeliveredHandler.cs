using MediatR;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Application.FinalizingShipment;

public class PackageDeliveredHandler : INotificationHandler<PackageDelivered>
{
    private readonly IEventProducer _eventProducer;

    public PackageDeliveredHandler(IEventProducer eventProducer)
    {
        _eventProducer = eventProducer;
    }

    public async Task Handle(PackageDelivered @event, CancellationToken cancellationToken)
    {
        // Notifying Order Saga
        await _eventProducer.PublishAsync(
            new OrderDelivered(
                @event.ShipmentId.Value,
                @event.OrderId.Value),
                cancellationToken
            );
    }
}