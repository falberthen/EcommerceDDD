namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class ProcessOrderHandler(
	IQuoteService quoteService,
	IEventStoreRepository<Order> orderWriteRepository,
	IEventBus eventPublisher
) : ICommandHandler<ProcessOrder>
{
	private readonly IQuoteService _quoteService = quoteService
		?? throw new ArgumentNullException(nameof(quoteService));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));
	private readonly IEventBus _eventPublisher = eventPublisher
		?? throw new ArgumentNullException(nameof(eventPublisher));

	public async Task<Result> HandleAsync(ProcessOrder command, CancellationToken cancellationToken)
	{
		Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());

		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Order {command.OrderId} not found.");

		// Idempotency: if already processed, re-publish OrderProcessed to retry the downstream chain
		if (order.Status == OrderStatus.Processed)
		{
			var orderLineDetails = order.OrderLines.Select(ol => new OrderLineDetails(
				ol.ProductItem.ProductId.Value,
				ol.ProductItem.ProductName,
				ol.ProductItem.UnitPrice.Amount,
				ol.ProductItem.Quantity)).ToList();

			var retryEvent = new OrderProcessed(
				order.CustomerId.Value,
				order.Id.Value,
				orderLineDetails,
				order.TotalPrice.Currency.Code,
				order.TotalPrice.Amount);

			await _eventPublisher.PublishEventAsync(retryEvent, cancellationToken);
			return Result.Ok();
		}

		if (order.Status != OrderStatus.Placed)
			return Result.Ok();

		// Getting open quote data
		var quoteResult = await GetQuoteAsync(command, cancellationToken);
		if (quoteResult.IsFailed)
			return Result.Fail(quoteResult.Errors);

		var quote = quoteResult.Value!;
		var quoteId = QuoteId.Of(quote.QuoteId!.Value);

		if (!quote.Items!.Any())
			return Result.Fail("No quote items found for customer.");

		var quoteItems = quote.Items!.Select(qi =>
			new ProductItemData()
			{
				ProductId = ProductId.Of(qi.ProductId!.Value),
				Quantity = qi.Quantity!.Value,
				ProductName = qi.ProductName!,
				UnitPrice = Money.Of(Convert.ToDecimal(qi.UnitPrice), quote.CurrencyCode!)
			}).ToList();

		var orderData = new OrderData(
			CustomerId.Of(quote.CustomerId!.Value),
			quoteId,
			Currency.OfCode(quote.CurrencyCode!),
			quoteItems);

		order.Process(orderData);

		var orderProcessedEvent = order.GetUncommittedEvents()
		   .OfType<OrderProcessed>()
		   .FirstOrDefault();

		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		await _eventPublisher
			.PublishEventAsync(orderProcessedEvent!, cancellationToken);

		return Result.Ok();
	}

	private async Task<Result<QuoteViewModel>> GetQuoteAsync(ProcessOrder command, CancellationToken cancellationToken)
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
				$"An error occurred processing order {command.OrderId}.");
		}
	}
}
