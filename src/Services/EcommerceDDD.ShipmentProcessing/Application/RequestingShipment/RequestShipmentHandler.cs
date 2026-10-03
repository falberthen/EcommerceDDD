namespace EcommerceDDD.ShipmentProcessing.Application.RequestingShipment;

public class RequestShipmentHandler(
	ICustomerManagementService customerManagementService,
	IOrderShipmentLookup orderShipmentLookup,
	IEventStoreRepository<Shipment> shipmentWriteRepository
)
{
	private readonly ICustomerManagementService _customerManagementService = customerManagementService;
	private readonly IOrderShipmentLookup _orderShipmentLookup = orderShipmentLookup;
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(RequestShipment command, CancellationToken cancellationToken)
    {
		// Requested by an earlier attempt.
		if (await _orderShipmentLookup.HasShipmentAsync(command.OrderId, cancellationToken))
			return Result.Ok();

        // Snapshot the customer's shipping address onto the shipment for audit.
        var shippingAddress = await _customerManagementService
            .GetShippingAddressAsync(command.CustomerId, cancellationToken);

        if (string.IsNullOrWhiteSpace(shippingAddress))
            return Result.Fail(new ValidationError($"Shipping address not found for customer {command.CustomerId}."));

        var shipmentData = new ShipmentData(command.OrderId, shippingAddress, command.ProductItems);
        var shipment = Shipment.Create(shipmentData);

		var shipmentCreatedEvent = shipment.GetUncommittedEvents()
			.OfType<ShipmentCreated>()
			.FirstOrDefault();

		// Committed with the shipment, so the shipment is guaranteed to be processed.
        await _shipmentWriteRepository
			.AppendEventsAndCommitAsync(shipment, cancellationToken, shipmentCreatedEvent!);

        return Result.Ok();
    }
}
