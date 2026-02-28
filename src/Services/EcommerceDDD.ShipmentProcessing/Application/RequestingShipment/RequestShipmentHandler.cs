using EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class RequestShipmentHandler(
	ICommandBus commandBus,
	IEventStoreRepository<Shipment> shipmentWriteRepository
) : ICommandHandler<RequestShipment>
{
	private readonly ICommandBus _commandBus = commandBus;
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(RequestShipment command, CancellationToken cancellationToken)
    {
        Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());
        var shipmentData = new ShipmentData(command.OrderId, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

        await _shipmentWriteRepository
			.AppendEventsAsync(shipment, cancellationToken);

        return await _commandBus
			.SendAsync(ProcessShipment.Create(shipment.Id), cancellationToken);
    }
}
