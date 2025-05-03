using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Payments.CancelingPayment;

public class RequestCancelPaymentHandler(ApiGatewayClient apiGatewayClient) : ICommandHandler<RequestCancelPayment>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;

	public async Task HandleAsync(RequestCancelPayment command, CancellationToken cancellationToken)
	{
		var cancelRequest = new CancelPaymentRequest()
		{
			PaymentCancellationReason = (int)command.PaymentCancellationReason
		};

		try
		{
			var paymentsRequestBuilder = _apiGatewayClient.Api.Payments[command.PaymentId.Value];
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