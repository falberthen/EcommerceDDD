namespace EcommerceDDD.PaymentProcessing.Application.CancelingPayment;

public record class CancelPayment : ICommand
{
	public PaymentId PaymentId { get; private set; }
	public PaymentCancellationReason PaymentCancellationReason { get; private set; }

	public static CancelPayment Create(
		PaymentId paymentId,
		int paymentCancellationReason)
	{
		if (paymentId is null)
			throw new ArgumentNullException(nameof(paymentId));

		return new CancelPayment(paymentId, (PaymentCancellationReason)paymentCancellationReason);
	}
	private CancelPayment(
		PaymentId paymentId,
		PaymentCancellationReason paymentCancellationReason)
	{
		PaymentId = paymentId;
		PaymentCancellationReason = paymentCancellationReason;
	}
}