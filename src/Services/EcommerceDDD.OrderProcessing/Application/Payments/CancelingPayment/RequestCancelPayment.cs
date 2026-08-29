namespace EcommerceDDD.OrderProcessing.Application.Payments.CancelingPayment;

public record class RequestCancelPayment : ICommand
{
	[Audit]
	public OrderId OrderId { get; private set; }
	public PaymentId PaymentId { get; private set; }
	public PaymentCancellationReason PaymentCancellationReason { get; private set; }

	public static RequestCancelPayment Create(
		OrderId orderId,
		PaymentId paymentId,
		PaymentCancellationReason paymentCancellationReason)
	{
		if (orderId is null)
			throw new ArgumentNullException(nameof(orderId));
		if (paymentId is null)
			throw new ArgumentNullException(nameof(paymentId));

		return new RequestCancelPayment(orderId, paymentId, paymentCancellationReason);
	}

	private RequestCancelPayment(
		OrderId orderId,
		PaymentId paymentId,
		PaymentCancellationReason paymentCancellationReason)
	{
		OrderId = orderId;
		PaymentId = paymentId;
		PaymentCancellationReason = paymentCancellationReason;
	}
}