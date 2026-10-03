namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class ProcessOrderHandler(
	IQuoteService quoteService,
	IProductInventoryHandler productInventoryHandler,
	IEventStoreRepository<Order> orderWriteRepository,
	IMessageBus messageBus
)
{
	private readonly IQuoteService _quoteService = quoteService
		?? throw new ArgumentNullException(nameof(quoteService));
	private readonly IProductInventoryHandler _productInventoryHandler = productInventoryHandler
		?? throw new ArgumentNullException(nameof(productInventoryHandler));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));
	private readonly IMessageBus _messageBus = messageBus
		?? throw new ArgumentNullException(nameof(messageBus));

	public async Task<Result> HandleAsync(ProcessOrder command, CancellationToken cancellationToken)
	{
		var order = await _orderWriteRepository
			.FetchForWritingAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			throw new RecordNotFoundException($"Order {command.OrderId} not found.");

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

			await _messageBus.PublishAsync(retryEvent);
			return Result.Ok();
		}

		if (order.Status != OrderStatus.Placed)
			return Result.Ok();

		// Confirmed before reading the items, so the order lines are the confirmed quote's.
		if (!await _quoteService.ConfirmQuoteAsync(command.QuoteId.Value, order.Id.Value, cancellationToken))
		{
			await _messageBus.PublishAsync(
				CancelOrder.Create(order.Id, OrderCancellationReason.QuoteUnavailable));
			return Result.Ok();
		}

		var quote = await _quoteService.GetQuoteDetailsAsync(command.QuoteId.Value, cancellationToken)
			?? throw new RecordNotFoundException($"Quote {command.QuoteId} not found.");
		var quoteId = QuoteId.Of(quote.QuoteId!.Value);

		if (!quote.Items!.Any())
			return Result.Fail(new ValidationError("No quote items found for customer."));

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

		// If any product is short, cancel the whole order (all-or-nothing). The decrement is idempotent per order,
		// so a retry of this handler never takes the stock twice.
		var inStock = await _productInventoryHandler.CheckProductsInStockAsync(quoteItems, cancellationToken);
		if (inStock && !await _productInventoryHandler.DecreaseQuantityInStockAsync(quoteItems, order.Id, cancellationToken))
		{
			// Another order took the stock between the check and the decrement, so some items may already be taken.			
			await _productInventoryHandler.IncreaseQuantityInStockAsync(quoteItems, order.Id, cancellationToken);
			inStock = false;
		}

		if (!inStock)
		{
			await _messageBus.PublishAsync(
				CancelOrder.Create(order.Id, OrderCancellationReason.ProductWasOutOfStock));
			return Result.Ok();
		}

		order.Process(orderData);

		var orderProcessedEvent = order.GetUncommittedEvents()
		   .OfType<OrderProcessed>()
		   .FirstOrDefault();

		// Committed with the order, so the saga is guaranteed to request the payment
		await _orderWriteRepository
			.AppendEventsAndCommitAsync(order, cancellationToken, orderProcessedEvent!);

		return Result.Ok();
	}
}
