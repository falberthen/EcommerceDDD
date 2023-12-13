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

    public async Task Handle(ShipPackage command, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentWriteRepository
            .FetchStreamAsync(command.ShipmentId.Value)
            ?? throw new RecordNotFoundException($"The shipment {command.ShipmentId} was not found.");

        shipment.RecordShipment();

        await _outboxMessageService.SaveAsOutboxMessageAsync(
            new PackageShipped(
                shipment.Id.Value,
                shipment.OrderId.Value,
                shipment.ShippedAt!.Value)
            );
        
        await _shipmentWriteRepository
            .AppendEventsAsync(shipment);
    }
}
