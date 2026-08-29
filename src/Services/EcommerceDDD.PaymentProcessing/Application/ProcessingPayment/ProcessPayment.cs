namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public record class ProcessPayment : ICommand
{
	public PaymentId PaymentId { get; private set; }
	[Audit]
	public OrderId OrderId { get; private set; }

	public static ProcessPayment Create(PaymentId paymentId, OrderId orderId)
	{
		if (paymentId is null)
			throw new ArgumentNullException(nameof(paymentId));
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));

		return new ProcessPayment(paymentId, orderId);
	}

	private ProcessPayment(PaymentId paymentId, OrderId orderId)
	{
		PaymentId = paymentId;
		OrderId = orderId;
	}
}
