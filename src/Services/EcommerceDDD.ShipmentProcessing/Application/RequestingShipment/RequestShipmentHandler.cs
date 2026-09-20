namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class RequestShipmentHandler(
	IMessageBus bus,
	ICustomerManagementService customerManagementService,
	IEventStoreRepository<Shipment> shipmentWriteRepository
)
{
	private readonly IMessageBus _bus = bus;
	private readonly ICustomerManagementService _customerManagementService = customerManagementService;
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(RequestShipment command, CancellationToken cancellationToken)
    {
        // Snapshot the customer's shipping address onto the shipment for audit.		
        var shippingAddress = await _customerManagementService
            .GetShippingAddressAsync(command.CustomerId, cancellationToken);

        if (string.IsNullOrWhiteSpace(shippingAddress))
            return Result.Fail($"Shipping address not found for customer {command.CustomerId}.");

        var shipmentData = new ShipmentData(command.OrderId, shippingAddress, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

        await _shipmentWriteRepository
			.AppendEventsAndCommitAsync(shipment, cancellationToken);

        return await _bus
			.InvokeAsync<Result>(ProcessShipment.Create(shipment.Id, command.OrderId), cancellationToken);
    }
}
