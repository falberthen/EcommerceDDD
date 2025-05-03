namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class RequestShipmentHandler(
	ICommandBus commandBus,
	IEventStoreRepository<Shipment> shipmentWriteRepository
) : ICommandHandler<RequestShipment>
{
	private readonly ICommandBus _commandBus = commandBus;
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task HandleAsync(RequestShipment command, CancellationToken cancellationToken)
    {
        var shipmentData = new ShipmentData(command.OrderId, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

        await _shipmentWriteRepository
			.AppendEventsAsync(shipment, cancellationToken);

        await _commandBus
			.SendAsync(ProcessShipment.Create(shipment.Id), cancellationToken);        
    }
}