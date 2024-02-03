namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class RequestShipmentHandler : ICommandHandler<RequestShipment>
{
    private readonly ICommandBus _commandBus;
    private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository;

    public RequestShipmentHandler(
        ICommandBus commandBus,
        IEventStoreRepository<Shipment> shipmentWriteRepository)
    {
        _commandBus = commandBus;        
        _shipmentWriteRepository = shipmentWriteRepository;        
    }

    public async Task Handle(RequestShipment command, CancellationToken cancellationToken)
    {
        var producIds = command.ProductItems
            .Select(pid => pid.ProductId.Value)
            .ToArray();

        var shipmentData = new ShipmentData(command.OrderId, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

        await _shipmentWriteRepository
            .AppendEventsAsync(shipment, cancellationToken);

        await _commandBus.SendAsync(ProcessShipment.Create(shipment.Id), cancellationToken);        
    }
}