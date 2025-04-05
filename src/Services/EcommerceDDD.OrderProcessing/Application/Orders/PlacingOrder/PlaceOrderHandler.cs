using System.Collections.Immutable;

namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class PlaceOrderHandler(
    IIntegrationHttpService integrationHttpService,
    IEventStoreRepository<Order> orderWriteRepository,
    IOrderStatusBroadcaster orderStatusBroadcaster,
    IConfiguration configuration) : ICommandHandler<PlaceOrder>
{
    private readonly IIntegrationHttpService _integrationHttpService = integrationHttpService;
    private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
    private readonly IConfiguration _configuration = configuration;

    public async Task Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        // Getting quote data
        var quote = await GetQuoteAsync(command)
            ?? throw new RecordNotFoundException($"No open quote found for customer.");
        
        // Confirming quote
        await ConfirmQuoteAsync(quote.QuoteId);

		// Placing order
		var oderItems = quote.Items.Select(qi => new ProductItemData()
		{
			ProductId = ProductId.Of(qi.ProductId),
			ProductName = qi.ProductName,
			Quantity = qi.Quantity,
			UnitPrice = Money.Of(qi.UnitPrice, quote.CurrencyCode)
		}).ToImmutableList();

		var orderData = new OrderData(
            CustomerId.Of(quote.CustomerId),
            QuoteId.Of(quote.QuoteId),
            Currency.OfCode(quote.CurrencyCode),
			oderItems);

        var order = Order.Place(orderData);

        // Appending to outbox for message broker
        var orderPlacedEvent = order.GetUncommittedEvents()
            .OfType<OrderPlaced>().FirstOrDefault();
        _orderWriteRepository.AppendToOutbox(orderPlacedEvent!);

        // Persisting aggregate
        await _orderWriteRepository
            .AppendEventsAsync(order);

        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                order.Id.Value,
                order.Status.ToString(),
                (int)order.Status));
    }

    private async Task<QuoteViewModelResponse> GetQuoteAsync(PlaceOrder command)
    {
        var apiRoute = _configuration["ApiRoutes:QuoteManagement"];
        var response = await _integrationHttpService.GetAsync<QuoteViewModelResponse>(
            $"{apiRoute}/{command.QuoteId.Value}/details")
            ?? throw new ApplicationLogicException(
                $"An error occurred retrieving quote {command.QuoteId}.");

        if (!response.Success)
            throw new ApplicationLogicException(response?.Message ?? string.Empty);

        var responseData = response.Data!;
        return responseData;
    }

    private async Task ConfirmQuoteAsync(Guid quoteId)
    {
        var apiRoute = _configuration["ApiRoutes:QuoteManagement"];
        var response = await _integrationHttpService.PutAsync(
            $"{apiRoute}/{quoteId}/confirm")
            ?? throw new ApplicationLogicException(
                $"An error occurred confirming quote {quoteId}.");

        if (!response.Success)
            throw new ApplicationLogicException(response?.Message ?? string.Empty);
    }
}

public record QuoteViewModelResponse(
    Guid QuoteId,
    Guid CustomerId,
    List<QuoteItemViewModel> Items,
    string CurrencyCode,
    decimal TotalPrice);

public record class QuoteItemViewModel(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);