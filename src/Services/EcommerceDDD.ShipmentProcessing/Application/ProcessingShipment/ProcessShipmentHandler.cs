namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

public class ProcessShipmentHandler(
	IEventStoreRepository<Shipment> shipmentWriteRepository
)
{
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(ProcessShipment command, CancellationToken cancellationToken)
	{
		var shipment = await _shipmentWriteRepository
				.FetchForWritingAsync(command.ShipmentId.Value, cancellationToken: cancellationToken);

		if (shipment is null)
			return Result.Fail($"The shipment {command.ShipmentId.Value} was not found.");

		INotification integrationEvent;
		var result = Result.Ok();
		
		try
		{
			shipment.Complete();

			integrationEvent = new ShipmentFinalized(
				shipment.Id.Value,
				shipment.OrderId.Value,
				shipment.ShippedAt!.Value);
		}
		catch (Exception)
		{
			shipment.Cancel(ShipmentCancellationReason.ProcessmentError);
			integrationEvent = new ShipmentFailed(shipment.Id.Value, shipment.OrderId.Value);

			result = Result.Fail($"An unexpected error occurred processing shipment {command.ShipmentId}.");
		}

		await _shipmentWriteRepository
			.AppendEventsAndCommitAsync(shipment, cancellationToken, integrationEvent);

		return result;
	}
}
