using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class PlaceOrderHandler(
	ApiGatewayClient apiGatewayClient,
	IEventStoreRepository<Order> orderWriteRepository,
	IOrderStatusBroadcaster orderStatusBroadcaster
) : ICommandHandler<PlaceOrder>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;
	private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;

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

		// Updating order status on the UI with SignalR
		await _orderStatusBroadcaster.UpdateOrderStatus(
			new UpdateOrderStatusRequest(
				order.CustomerId.Value,
				order.Id.Value,
				order.Status.ToString(),
				(int)order.Status));
	}

	private async Task<QuoteViewModel> GetQuoteAsync(PlaceOrder command, CancellationToken cancellationToken)
	{
		var quoteRequestBuilder = _apiGatewayClient.Api.Quotes[command.QuoteId.Value];
		var response = await quoteRequestBuilder.Details
			.GetAsync(cancellationToken: cancellationToken);

		if (response?.Data is null)
			throw new ApplicationLogicException(response?.Message ?? string.Empty);

		return response.Data;
	}

	private async Task ConfirmQuoteAsync(Guid quoteId, CancellationToken cancellationToken)
	{
		var quoteRequestBuilder = _apiGatewayClient.Api.Quotes[quoteId];
		await quoteRequestBuilder.Confirm
			.PutAsync(cancellationToken: cancellationToken);
	}
}