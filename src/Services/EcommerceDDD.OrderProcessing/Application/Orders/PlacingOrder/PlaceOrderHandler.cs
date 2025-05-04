using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class PlaceOrderHandler(
	ApiGatewayClient apiGatewayClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<PlaceOrder>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient
		?? throw new ArgumentNullException(nameof(apiGatewayClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task HandleAsync(PlaceOrder command, CancellationToken cancellationToken)
	{
		// Getting quote data
		QuoteViewModel quote = await GetQuoteAsync(command, cancellationToken)
			?? throw new RecordNotFoundException($"No open quote found for customer.");
		if (!quote.Items!.Any())
			throw new RecordNotFoundException($"No quote items found for customer.");

		// Confirming quote
		await ConfirmQuoteAsync(quote.QuoteId!.Value, cancellationToken);

		// Placing order
		var oderItems = quote.Items!.Select(qi => new ProductItemData()
		{
			ProductId = ProductId.Of(qi.ProductId!.Value),
			ProductName = qi.ProductName!,
			Quantity = qi.Quantity!.Value,
			UnitPrice = Money.Of(Convert.ToDecimal(qi.UnitPrice!.Value), quote.CurrencyCode!)
		}).ToImmutableList();

		var orderData = new OrderData(
			CustomerId.Of(quote.CustomerId!.Value),
			QuoteId.Of(quote.QuoteId.Value),
			Currency.OfCode(quote.CurrencyCode!),
			oderItems);

		var order = Order.Place(orderData);

		// Appending to outbox for message broker
		var orderPlacedEvent = order.GetUncommittedEvents()
			.OfType<OrderPlaced>().FirstOrDefault();
		_orderWriteRepository.AppendToOutbox(orderPlacedEvent!);

		// Persisting aggregate
		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		try
		{
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

	private async Task<QuoteViewModel> GetQuoteAsync(PlaceOrder command, CancellationToken cancellationToken)
	{
		try
		{
			var quoteRequestBuilder = _apiGatewayClient.Api.V2.Quotes[command.QuoteId.Value];
			var response = await quoteRequestBuilder.Details
				.GetAsync(cancellationToken: cancellationToken);

			if (response?.Data is null)
				throw new ApplicationLogicException(response?.Message ?? string.Empty);

			return response.Data;
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred when getting quote {command.QuoteId.Value}.", ex);
		}
	}

	private async Task ConfirmQuoteAsync(Guid quoteId, CancellationToken cancellationToken)
	{
		try
		{
			var quoteRequestBuilder = _apiGatewayClient.Api.V2.Quotes[quoteId];
			await quoteRequestBuilder.Confirm
				.PutAsync(cancellationToken: cancellationToken);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred when confirming quote {quoteId}.", ex);
		}
	}
}