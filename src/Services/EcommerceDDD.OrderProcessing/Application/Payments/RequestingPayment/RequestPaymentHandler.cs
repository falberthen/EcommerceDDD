using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Payments.RequestingPayment;

public class RequestPaymentHandler(
	ApiGatewayClient apiGatewayClient,
	IOrderStatusBroadcaster orderStatusBroadcaster,
    IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestPayment>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;
	private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;

	public async Task HandleAsync(RequestPayment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

		var paymentRequest = new PaymentRequest()
		{
			CurrencyCode = command.Currency.Code,
			CustomerId = command.CustomerId.Value,
			OrderId = command.OrderId.Value,
			TotalAmount = Convert.ToDouble(command.TotalPrice.Amount)
		};

		try
		{
			var paymentsRequestBuilder = _apiGatewayClient.Api.Payments;
			await paymentsRequestBuilder
				.PostAsync(paymentRequest, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException($"An error occurred requesting payment for order {command.OrderId}.");
		}
		
        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                command.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}