namespace EcommerceDDD.OrderProcessing.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler(
	IPaymentService paymentService
)
{
	private readonly IPaymentService _paymentService = paymentService
		?? throw new ArgumentNullException(nameof(paymentService));

	public async Task<Result> HandleAsync(RequestCancelPayment command, CancellationToken cancellationToken)
	{
		await _paymentService.CancelPaymentAsync(
			command.OrderId.Value,
			command.PaymentId.Value,
			(int)command.PaymentCancellationReason,
			cancellationToken);

		return Result.Ok();
	}
}
