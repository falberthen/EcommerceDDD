using EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingPayment.IntegrationEvents;

public class ProcessShipmentHandler(
	IEventStoreRepository<Shipment> shipmentWriteRepository
) : ICommandHandler<ProcessShipment>
{
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task HandleAsync(ProcessShipment command, CancellationToken cancellationToken)
	{
		var shipment = await _shipmentWriteRepository
				.FetchStreamAsync(command.ShipmentId.Value, cancellationToken: cancellationToken)
				?? throw new RecordNotFoundException($"The shipment {command.ShipmentId} was not found.");

		try
		{						
			// Completing shipment
			shipment.Complete();

			// Appending integration event to outbox            
			_shipmentWriteRepository.AppendToOutbox(
				new ShipmentFinalized(
					shipment.Id.Value,
					shipment.OrderId.Value,
					shipment.ShippedAt!.Value));

			// Persisting aggregate
			await _shipmentWriteRepository
				.AppendEventsAsync(shipment, cancellationToken);
		}
		catch (Exception) // unexpected issue
		{
			shipment.Cancel(ShipmentCancellationReason.ProcessmentError);

			// Appending integration event to outbox
			_shipmentWriteRepository.AppendToOutbox(
				new ShipmentFailed(shipment.Id.Value, shipment.OrderId.Value));

			// Persisting domain event
			await _shipmentWriteRepository
				.AppendEventsAsync(shipment, cancellationToken);
		}
	}
}