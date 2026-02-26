namespace EcommerceDDD.OrderProcessing.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler(
	IPaymentService paymentService
) : ICommandHandler<RequestCancelPayment>
{
	private readonly IPaymentService _paymentService = paymentService
		?? throw new ArgumentNullException(nameof(paymentService));

	public async Task<Result> HandleAsync(RequestCancelPayment command, CancellationToken cancellationToken)
	{
		try
		{
			await _paymentService.CancelPaymentAsync(
				command.PaymentId.Value,
				(int)command.PaymentCancellationReason,
				cancellationToken);

			return Result.Ok();
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred requesting cancelling payment {command.PaymentId.Value}.");
		}
	}
}
