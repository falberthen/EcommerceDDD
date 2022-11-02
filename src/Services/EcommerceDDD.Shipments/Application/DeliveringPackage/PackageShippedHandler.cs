using MediatR;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Application.DeliveringPackage;

public class PackageShippedHandler : INotificationHandler<PackageShipped>
{
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;

    public PackageShippedHandler(IEventStoreRepository<Shipment> shipmentWriteRepository)
    {
        _shipmentWriteRepository = shipmentWriteRepository;
    }

    public async Task Handle(PackageShipped @event, CancellationToken cancellationToken)
    {       
        var shipment = await _shipmentWriteRepository
            .FetchStreamAsync(@event.ShipmentId);

        shipment.RecordDelivery();

        await _shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);
    }
}