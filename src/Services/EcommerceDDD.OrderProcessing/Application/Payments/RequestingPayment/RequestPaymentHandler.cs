using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Payments.RequestingPayment;

public class RequestPaymentHandler(
	ApiGatewayClient apiGatewayClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestPayment>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient
		?? throw new ArgumentNullException(nameof(apiGatewayClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task HandleAsync(RequestPayment command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
			?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

		var productItemsRequest = order.OrderLines
			.Select(ol => new ProductItemRequest()
			{
				ProductId = ol.ProductItem.ProductId.Value,
				ProductName = ol.ProductItem.ProductName,
				Quantity = ol.ProductItem.Quantity,
				UnitPrice = Convert.ToDouble(ol.ProductItem.UnitPrice.Amount)
			}).ToList();

		// Requesting payment
		await RequestPaymentAsync(command, productItemsRequest, cancellationToken);

		try
		{
			await Task.Delay(TimeSpan.FromSeconds(5)); // 5-second delay

			// Updating order status on the UI with SignalR
			var request = new UpdateOrderStatusRequest()
			{
				CustomerId = order.CustomerId.Value,
				OrderId = order.Id.Value,
				OrderStatusText = order.Status.ToString(),
				OrderStatusCode = (int)order.Status
			};

			await _apiGatewayClient.Api.V2.Signalr.Updateorderstatus
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred when updating status for order {order.Id.Value}.", ex);
		}
	}

	public async Task RequestPaymentAsync(RequestPayment command, List<ProductItemRequest> productItemsRequest,
		CancellationToken cancellationToken)
	{
		try
		{
			var paymentRequest = new PaymentRequest()
			{
				CurrencyCode = command.Currency.Code,
				CustomerId = command.CustomerId.Value,
				OrderId = command.OrderId.Value,
				TotalAmount = Convert.ToDouble(command.TotalPrice.Amount),
				ProductItems = productItemsRequest
			};

			var paymentsRequestBuilder = _apiGatewayClient.Api.V2.Payments;
			await paymentsRequestBuilder
				.PostAsync(paymentRequest, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException($"An error occurred requesting payment for order {command.OrderId}.", ex);
		}
	}
}