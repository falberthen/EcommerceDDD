using MediatR;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Shipments.Application.DeliveringPackage;

public class PackageShippedHandler : INotificationHandler<DomainEventNotification<PackageShipped>>
{
    private readonly IServiceProvider _serviceProvider;

    public PackageShippedHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(DomainEventNotification<PackageShipped> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;
        using var scopedService = _serviceProvider.CreateScope();
        var shipmentWriteRepository = scopedService
           .ServiceProvider.GetRequiredService<IEventStoreRepository<Shipment>>();

        var shipment = await shipmentWriteRepository
            .FetchStream(@event.ShipmentId.Value);

        shipment.RecordDelivery();

        await shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);
    }
}