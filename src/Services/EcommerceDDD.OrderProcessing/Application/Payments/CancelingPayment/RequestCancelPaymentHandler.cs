namespace EcommerceDDD.OrderProcessing.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler(PaymentProcessingClient paymentProcessingClient) : ICommandHandler<RequestCancelPayment>
{
	private readonly PaymentProcessingClient _paymentProcessingClient = paymentProcessingClient
		?? throw new ArgumentNullException(nameof(paymentProcessingClient));

	public async Task HandleAsync(RequestCancelPayment command, CancellationToken cancellationToken)
	{
		var cancelRequest = new CancelPaymentRequest()
		{
			PaymentCancellationReason = (int)command.PaymentCancellationReason
		};

		try
		{
			var paymentsRequestBuilder = _paymentProcessingClient.Api.V2.Payments[command.PaymentId.Value];
			await paymentsRequestBuilder
				.DeleteAsync(cancelRequest, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred requesting cancelling payment {command.PaymentId.Value}.", ex);
		}
	}
}