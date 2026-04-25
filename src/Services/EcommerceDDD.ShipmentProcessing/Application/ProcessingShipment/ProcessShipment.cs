namespace EcommerceDDD.ShipmentProcessing.Application.ProcessingShipment;

public record class ProcessShipment : ICommand, ITraceable
{
	public ShipmentId ShipmentId { get; private set; }
	public OrderId OrderId { get; private set; }

	public static ProcessShipment Create(ShipmentId shipmentId, OrderId orderId)
	{
		if (shipmentId is null)
			throw new ArgumentNullException(nameof(shipmentId));
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new ProcessShipment(shipmentId, orderId);
	}

	public IEnumerable<KeyValuePair<string, object>> GetSpanTags() =>
		[new(TelemetryTags.OrderId, OrderId.Value)];

	private ProcessShipment(ShipmentId shipmentId, OrderId orderId)
	{
		ShipmentId = shipmentId;
		OrderId = orderId;
	}
}
