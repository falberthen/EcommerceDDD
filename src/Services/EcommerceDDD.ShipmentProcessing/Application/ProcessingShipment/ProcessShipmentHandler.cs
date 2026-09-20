namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

public class ProcessShipmentHandler(
	IConfiguration configuration,
	IEventStoreRepository<Shipment> shipmentWriteRepository
)
{
	private readonly IConfiguration _configuration = configuration;
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(ProcessShipment command, CancellationToken cancellationToken)
	{
		var shipment = await _shipmentWriteRepository
				.FetchForWritingAsync(command.ShipmentId.Value, cancellationToken: cancellationToken);

		if (shipment is null)
			return Result.Fail($"The shipment {command.ShipmentId.Value} was not found.");

		INotification integrationEvent;

		// Demo simulation: the carrier can't deliver to an invalid delivery address.
		if (IsUndeliverable(shipment.ShippingAddress))
		{
			shipment.Cancel(ShipmentCancellationReason.Undeliverable);
			integrationEvent = new ShipmentNotDelivered(shipment.Id.Value, shipment.OrderId.Value);
		}
		else
		{
			shipment.Complete();
			integrationEvent = new ShipmentFinalized(
				shipment.Id.Value,
				shipment.OrderId.Value,
				shipment.ShippedAt!.Value);
		}

		await _shipmentWriteRepository
			.AppendEventsAndCommitAsync(shipment, cancellationToken, integrationEvent);

		return Result.Ok();
	}

	private bool IsUndeliverable(string shippingAddress)
	{
		var marker = _configuration["ShipmentFailureSimulation:UndeliverableAddressMarker"];
		return !string.IsNullOrWhiteSpace(marker)
			&& shippingAddress.Contains(marker, StringComparison.OrdinalIgnoreCase);
	}
}
