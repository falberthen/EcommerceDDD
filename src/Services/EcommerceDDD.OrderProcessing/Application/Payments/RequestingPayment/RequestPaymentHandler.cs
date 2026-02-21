using ProductItemRequest = EcommerceDDD.ServiceClients.PaymentProcessing.Models.ProductItemRequest;

namespace EcommerceDDD.OrderProcessing.Application.Payments.RequestingPayment;

public class RequestPaymentHandler(
	SignalRClient signalrClient,
	PaymentProcessingClient paymentProcessingClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RequestPayment>
{
	private readonly SignalRClient _signalrClient = signalrClient
		?? throw new ArgumentNullException(nameof(signalrClient));
	private readonly PaymentProcessingClient _paymentProcessingClient = paymentProcessingClient
		?? throw new ArgumentNullException(nameof(paymentProcessingClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(RequestPayment command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Failed to find the order {command.OrderId}.");

		var productItemsRequest = order.OrderLines
			.Select(ol => new ProductItemRequest()
			{
				ProductId = ol.ProductItem.ProductId.Value,
				ProductName = ol.ProductItem.ProductName,
				Quantity = ol.ProductItem.Quantity,
				UnitPrice = Convert.ToDouble(ol.ProductItem.UnitPrice.Amount)
			}).ToList();

		var paymentResult = await RequestPaymentAsync(command, productItemsRequest, cancellationToken);
		if (paymentResult.IsFailed)
			return paymentResult;

		try
		{
			await Task.Delay(TimeSpan.FromSeconds(5));

			var request = new UpdateOrderStatusRequest()
			{
				CustomerId = order.CustomerId.Value,
				OrderId = order.Id.Value,
				OrderStatusText = order.Status.ToString(),
				OrderStatusCode = (int)order.Status
			};

			await _signalrClient.Api.V2.Signalr.Updateorderstatus
				.PostAsync(request, cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail($"An error occurred when updating status for order {order.Id.Value}.");
		}

		return Result.Ok();
	}

	private async Task<Result> RequestPaymentAsync(RequestPayment command, List<ProductItemRequest> productItemsRequest,
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

			var paymentsRequestBuilder = _paymentProcessingClient.Api.V2.Payments;
			await paymentsRequestBuilder
				.PostAsync(paymentRequest, cancellationToken: cancellationToken);

			return Result.Ok();
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail($"An error occurred requesting payment for order {command.OrderId}.");
		}
	}
}
