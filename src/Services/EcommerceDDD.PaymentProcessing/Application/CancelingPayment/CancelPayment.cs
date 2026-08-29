namespace EcommerceDDD.PaymentProcessing.Application.CancelingPayment;

public record class CancelPayment : ICommand
{
	[Audit]
	public OrderId OrderId { get; private set; }
	public PaymentId PaymentId { get; private set; }
	public PaymentCancellationReason PaymentCancellationReason { get; private set; }

	public static CancelPayment Create(
		OrderId orderId,
		PaymentId paymentId,
		int paymentCancellationReason)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));
		if (paymentId is null)
			throw new ArgumentNullException(nameof(paymentId));

		return new CancelPayment(orderId, paymentId, (PaymentCancellationReason)paymentCancellationReason);
	}

	private CancelPayment(
		OrderId orderId,
		PaymentId paymentId,
		PaymentCancellationReason paymentCancellationReason)
	{
		OrderId = orderId;
		PaymentId = paymentId;
		PaymentCancellationReason = paymentCancellationReason;
	}
}