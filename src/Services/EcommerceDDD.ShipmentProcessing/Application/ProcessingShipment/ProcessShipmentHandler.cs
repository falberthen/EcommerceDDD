using EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingPayment.IntegrationEvents;

public class ProcessShipmentHandler(
	IEventStoreRepository<Shipment> shipmentWriteRepository
) : ICommandHandler<ProcessShipment>
{
	private readonly IEventStoreRepository<Shipment> _shipmentWriteRepository = shipmentWriteRepository;

	public async Task<Result> HandleAsync(ProcessShipment command, CancellationToken cancellationToken)
	{
		var shipment = await _shipmentWriteRepository
				.FetchStreamAsync(command.ShipmentId.Value, cancellationToken: cancellationToken);

		if (shipment is null)
			return Result.Fail($"The shipment {command.ShipmentId} was not found.");

		try
		{
			shipment.Complete();

			_shipmentWriteRepository.AppendToOutbox(
				new ShipmentFinalized(
					shipment.Id.Value,
					shipment.OrderId.Value,
					shipment.ShippedAt!.Value));

			await _shipmentWriteRepository
				.AppendEventsAsync(shipment, cancellationToken);

			return Result.Ok();
		}
		catch (Exception)
		{
			shipment.Cancel(ShipmentCancellationReason.ProcessmentError);

			_shipmentWriteRepository.AppendToOutbox(
				new ShipmentFailed(shipment.Id.Value, shipment.OrderId.Value));

			await _shipmentWriteRepository
				.AppendEventsAsync(shipment, cancellationToken);

			return Result.Fail($"An unexpected error occurred processing shipment {command.ShipmentId}.");
		}
	}
}
