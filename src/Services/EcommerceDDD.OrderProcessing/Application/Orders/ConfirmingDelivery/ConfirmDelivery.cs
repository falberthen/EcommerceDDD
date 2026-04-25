namespace EcommerceDDD.OrderProcessing.Application.Orders.ConfirmingDelivery;

public record class ConfirmDelivery : ICommand, ITraceable
{
	public OrderId OrderId { get; private set; }

	public static ConfirmDelivery Create(OrderId orderId)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new ConfirmDelivery(orderId);
	}

	public IEnumerable<KeyValuePair<string, object>> GetSpanTags() =>
		[new(TelemetryTags.OrderId, OrderId.Value)];

	private ConfirmDelivery(OrderId orderId)
	{
		OrderId = orderId;
	}
}
