using EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class RequestShipmentHandler(
	IMessageBus bus,
	IEventStoreRepository<Shipment> shipmentWriteRepository
)
{
	private readonly IMessageBus _bus = bus;
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(RequestShipment command, CancellationToken cancellationToken)
    {
        var shipmentData = new ShipmentData(command.OrderId, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

        await _shipmentWriteRepository
			.AppendEventsAsync(shipment, cancellationToken);

        return await _bus
			.InvokeAsync<Result>(ProcessShipment.Create(shipment.Id, command.OrderId), cancellationToken);
    }
}
