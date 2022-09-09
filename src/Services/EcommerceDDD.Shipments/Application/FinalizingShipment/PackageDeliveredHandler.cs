using MediatR;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Application.FinalizingShipment;

public class PackageDeliveredHandler : INotificationHandler<DomainEventNotification<PackageDelivered>>
{
    private readonly IServiceProvider _serviceProvider;

    public PackageDeliveredHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(DomainEventNotification<PackageDelivered> notification, CancellationToken cancellationToken)
    {
        using var scopedService = _serviceProvider.CreateScope();
        var eventProducer = scopedService
           .ServiceProvider.GetRequiredService<IEventProducer>();

        var @event = notification.DomainEvent;

        // Notifying Order Saga
        await eventProducer.PublishAsync(
            new OrderDelivered(
                @event.ShipmentId.Value,
                @event.OrderId.Value),
                cancellationToken
            );
    }
}