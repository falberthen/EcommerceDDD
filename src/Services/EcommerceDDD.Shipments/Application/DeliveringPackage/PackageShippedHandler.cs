using MediatR;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Shipments.Domain.Events;

namespace EcommerceDDD.Shipments.Application.DeliveringPackage;

public class PackageShippedHandler : INotificationHandler<PackageShipped>
{
    private readonly IMediator _mediator;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;

    public PackageShippedHandler(
        IMediator mediator,
        IEventStoreRepository<Shipment> shipmentWriteRepository)
    {
        _mediator = mediator;
        _shipmentWriteRepository = shipmentWriteRepository;
    }

    public async Task Handle(PackageShipped @event, CancellationToken cancellationToken)
    {
        // Delivery happened magically well
        var shipment = await _shipmentWriteRepository
            .FetchStream(@event.ShipmentId.Value);

        var packageDeliveredEvent = shipment.RecordDelivery();
        
        await _mediator.Publish(packageDeliveredEvent);
    }
}