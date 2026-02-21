namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class ProcessOrderHandler(
	QuoteManagementClient quoteManagementClient,
	IEventStoreRepository<Order> orderWriteRepository,
	IEventBus eventPublisher
) : ICommandHandler<ProcessOrder>
{
	private readonly QuoteManagementClient _quoteManagementClient = quoteManagementClient
		?? throw new ArgumentNullException(nameof(quoteManagementClient));
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository
		?? throw new ArgumentNullException(nameof(orderWriteRepository));
	private readonly IEventBus _eventPublisher = eventPublisher
		?? throw new ArgumentNullException(nameof(eventPublisher));

	public async Task<Result> HandleAsync(ProcessOrder command, CancellationToken cancellationToken)
	{
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

		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken);

		if (order is null)
			return Result.Fail($"Order {command.OrderId} not found.");

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
			var quoteRequestBuilder = _quoteManagementClient.Api.V2.Quotes[command.QuoteId.Value];
			var response = await quoteRequestBuilder
				.Details.GetAsync(cancellationToken: cancellationToken);

			if (response is null)
				return Result.Fail<QuoteViewModel>(
					new RecordNotFoundError($"Quote data not found."));

			return Result.Ok(response);
		}
		catch (Microsoft.Kiota.Abstractions.ApiException)
		{
			return Result.Fail<QuoteViewModel>(
				$"An error occurred processing order {command.OrderId}.");
		}
	}
}
