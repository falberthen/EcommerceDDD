namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class PlaceOrderHandler(
	IOrderNotificationService orderNotificationService,
	IQuoteService quoteService,
	IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<PlaceOrder>
{
	private readonly IOrderNotificationService _orderNotificationService = orderNotificationService
		?? throw new ArgumentNullException(nameof(orderNotificationService));
	private readonly IQuoteService _quoteService = quoteService
		?? throw new ArgumentNullException(nameof(quoteService));
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
			await _orderNotificationService.UpdateOrderStatusAsync(
				order.CustomerId.Value,
				order.Id.Value,
				order.Status.ToString(),
				(int)order.Status,
				cancellationToken);
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred when updating status for order {order.Id.Value}.");
		}

		return Result.Ok();
	}

	private async Task<Result<QuoteViewModel>> GetQuoteAsync(PlaceOrder command, CancellationToken cancellationToken)
	{
		try
		{
			var response = await _quoteService
				.GetQuoteDetailsAsync(command.QuoteId.Value, cancellationToken);

			if (response is null)
				return Result.Fail<QuoteViewModel>(
					new RecordNotFoundError($"Quote data not found."));

			return Result.Ok(response);
		}
		catch (Exception)
		{
			return Result.Fail<QuoteViewModel>(
				$"An error occurred when getting quote {command.QuoteId.Value}.");
		}
	}

	private async Task<Result> ConfirmQuoteAsync(Guid quoteId, CancellationToken cancellationToken)
	{
		try
		{
			await _quoteService.ConfirmQuoteAsync(quoteId, cancellationToken);
			return Result.Ok();
		}
		catch (Exception)
		{
			return Result.Fail($"An error occurred when confirming quote {quoteId}.");
		}
	}
}
