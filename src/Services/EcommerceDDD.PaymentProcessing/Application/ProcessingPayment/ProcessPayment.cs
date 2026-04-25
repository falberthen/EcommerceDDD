namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public record class ProcessPayment : ICommand, ITraceable
{
	public PaymentId PaymentId { get; private set; }
	public OrderId OrderId { get; private set; }

	public static ProcessPayment Create(PaymentId paymentId, OrderId orderId)
	{
		if (paymentId is null)
			throw new ArgumentNullException(nameof(paymentId));
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new ProcessPayment(paymentId, orderId);
	}

	public IEnumerable<KeyValuePair<string, object>> GetSpanTags() =>
		[new(TelemetryTags.OrderId, OrderId.Value)];

	private ProcessPayment(PaymentId paymentId, OrderId orderId)
	{
		PaymentId = paymentId;
		OrderId = orderId;
	}
}
