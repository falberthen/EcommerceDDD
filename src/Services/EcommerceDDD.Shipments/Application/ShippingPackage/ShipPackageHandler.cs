using MediatR;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Core.Testing;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public class ShipPackageHandler : CommandHandler<ShipPackage>
{
    private readonly IMediator _mediator;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;

    public ShipPackageHandler(
        IMediator mediator,
        IEventStoreRepository<Shipment> shipmentWriteRepository)
    {
        _mediator = mediator;
        _shipmentWriteRepository = shipmentWriteRepository;
    }

    public override async Task Handle(ShipPackage command, CancellationToken cancellationToken)
    {
        // Pretenting requesting shipping the package with the carrier...
        // 1..2..3

        var shipment = Shipment
            .CreateNew(command.OrderId, command.ProductItems);

        var @event = shipment.GetUncommittedEvents()
            .FirstOrDefault(e => e.GetType() == typeof(PackageShipped));

        await _shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);
        
        await _mediator.Publish(@event);
    }
}