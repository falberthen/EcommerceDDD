namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class PlaceOrderHandler(
	SignalRClient signalrClient,
	QuoteManagementClient quoteManagementClient,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<PlaceOrder>
{
	private readonly SignalRClient _signalrClient = signalrClient
		?? throw new ArgumentNullException(nameof(signalrClient));
	private readonly QuoteManagementClient _quoteManagementClient = quoteManagementClient
		?? throw new ArgumentNullException(nameof(quoteManagementClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));

	public async Task<Result> HandleAsync(PlaceOrder command, CancellationToken cancellationToken)
	{
		var quoteResult = await GetQuoteAsync(command, cancellationToken);
		if (quoteResult.IsFailed)
			return Result.Fail(quoteResult.Errors);

		var quote = quoteResult.Value!;

		if (!quote.Items!.Any())
			return Result.Fail("No quote items found for customer.");

		var confirmResult = await ConfirmQuoteAsync(quote.QuoteId!.Value, cancellationToken);
		if (confirmResult.IsFailed)
			return confirmResult;

		var orderItems = quote.Items!.Select(qi => new ProductItemData()
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
			orderItems);

		var order = Order.Place(orderData);

		var orderPlacedEvent = order.GetUncommittedEvents()
			.OfType<OrderPlaced>().FirstOrDefault();
		_orderWriteRepository.AppendToOutbox(orderPlacedEvent!);

		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		try
		{
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

	private async Task<Result<QuoteViewModel>> GetQuoteAsync(PlaceOrder command, CancellationToken cancellationToken)
	{
		try
		{
			var quoteRequestBuilder = _quoteManagementClient.Api.V2.Internal.Quotes[command.QuoteId.Value];
			var response = await quoteRequestBuilder.Details
				.GetAsync(cancellationToken: cancellationToken);

			if (response is null)
				return Result.Fail<QuoteViewModel>(
					new RecordNotFoundError($"Quote data not found."));

			return Result.Ok(response);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail<QuoteViewModel>(
				$"An error occurred when getting quote {command.QuoteId.Value}.");
		}
	}

	private async Task<Result> ConfirmQuoteAsync(Guid quoteId, CancellationToken cancellationToken)
	{
		try
		{
			var quoteRequestBuilder = _quoteManagementClient.Api.V2.Internal.Quotes[quoteId];
			await quoteRequestBuilder.Confirm
				.PutAsync(cancellationToken: cancellationToken);

			return Result.Ok();
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail($"An error occurred when confirming quote {quoteId}.");
		}
	}
}
