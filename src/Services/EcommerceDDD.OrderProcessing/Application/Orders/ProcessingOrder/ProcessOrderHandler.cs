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

	public async Task HandleAsync(ProcessOrder command, CancellationToken cancellationToken)
	{
		// Getting open quote data
		QuoteViewModel quote = await GetQuoteAsync(command, cancellationToken)
			?? throw new RecordNotFoundException($"No open quote found for customer {command.CustomerId}.");
		var quoteId = QuoteId.Of(quote.QuoteId!.Value);

		if (!quote.Items!.Any())
			throw new RecordNotFoundException($"No quote items found for customer.");

		// Building Order data
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

		// Processing order
		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
			?? throw new RecordNotFoundException($"Order {command.OrderId} not found.");

		order.Process(orderData);

		// Keeping event for publishing
		var orderProcessedEvent = order.GetUncommittedEvents()
		   .OfType<OrderProcessed>()
		   .FirstOrDefault();

		// Persisting domain event
		await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

		// publishing event
		await _eventPublisher
			.PublishEventAsync(orderProcessedEvent!, cancellationToken);
	}

	private async Task<QuoteViewModel> GetQuoteAsync(ProcessOrder command, CancellationToken cancellationToken)
	{
		try
		{
			var quoteRequestBuilder = _quoteManagementClient.Api.V2.Quotes[command.QuoteId.Value];
			var response = await quoteRequestBuilder
				.Details.GetAsync(cancellationToken: cancellationToken);

			if (response?.Data is null)
				throw new ApplicationLogicException(response?.Message ?? string.Empty);

			return response.Data;
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred processing order {command.OrderId}.", ex);
		}		
	}
}