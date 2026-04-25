namespace EcommerceDDD.OrderProcessing.Application.Orders.CancelingOrder;

public record class CancelOrder : ICommand, ITraceable
{
	public OrderId OrderId { get; private set; }
	public OrderCancellationReason CancellationReason { get; private set; }

	public static CancelOrder Create(
	   OrderId orderId,
	   OrderCancellationReason CancellationReason)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new CancelOrder(orderId, CancellationReason);
	}

	public IEnumerable<KeyValuePair<string, object>> GetSpanTags() =>
		[new(TelemetryTags.OrderId, OrderId.Value)];

	private CancelOrder(
		OrderId orderId,
		OrderCancellationReason cancellationReason)
	{
		OrderId = orderId;
		CancellationReason = cancellationReason;
	}
}