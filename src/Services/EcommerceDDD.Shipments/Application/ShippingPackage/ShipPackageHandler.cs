using MediatR;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public class ShipPackageHandler : ICommandHandler<ShipPackage>
{
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;
    private readonly IOutboxMessageService _outboxMessageService;

    public ShipPackageHandler(
        IEventStoreRepository<Shipment> shipmentWriteRepository,
        IOutboxMessageService outboxMessageService)
    {
        _shipmentWriteRepository = shipmentWriteRepository;
        _outboxMessageService = outboxMessageService;
    }

    public async Task<Unit> Handle(ShipPackage command, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentWriteRepository
            .FetchStreamAsync(command.ShipmentId.Value);

        shipment.RecordShipment();
        
        await _outboxMessageService.SaveAsOutboxMessageAsync(
            new PackageShipped(
                shipment.Id.Value,
                shipment.OrderId.Value,
                shipment.ShippedAt!.Value)
            );

        await _shipmentWriteRepository
            .AppendEventsAsync(shipment);

        return Unit.Value;
    }
}
