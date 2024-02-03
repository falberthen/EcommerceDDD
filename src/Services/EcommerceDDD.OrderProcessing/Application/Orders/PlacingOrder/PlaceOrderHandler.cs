namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster;
    private readonly IConfiguration _configuration;

    public PlaceOrderHandler(
        IIntegrationHttpService integrationHttpService,
        IEventStoreRepository<Order> orderWriteRepository,
        IOrderStatusBroadcaster orderStatusBroadcaster,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _orderWriteRepository = orderWriteRepository;
        _orderStatusBroadcaster = orderStatusBroadcaster;
        _configuration = configuration;
    }

    public async Task Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        // Getting quote data
        var quote = await GetQuote(command)
            ?? throw new RecordNotFoundException($"No open quote found for customer {command.CustomerId}.");
        
        // Confirming quote
        await ConfirmQuote(quote.QuoteId);

        // Placing order
        var orderData = new OrderData(
            CustomerId.Of(quote.CustomerId),
            QuoteId.Of(quote.QuoteId),
            Currency.OfCode(quote.CurrencyCode));

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

    private async Task<QuoteViewModelResponse> GetQuote(PlaceOrder command)
    {
        var apiRoute = _configuration["ApiRoutes:QuoteManagement"];
        var response = await _integrationHttpService.GetAsync<QuoteViewModelResponse>(
            $"{apiRoute}/{command.CustomerId.Value}/quote/{command.QuoteId.Value}")
            ?? throw new ApplicationLogicException(
                $"An error occurred retrieving quote for customer {command.CustomerId.Value}.");

        if (!response.Success)
            throw new ApplicationLogicException(response?.Message ?? string.Empty);

        var responseData = response.Data!;
        return responseData;
    }

    private async Task ConfirmQuote(Guid quoteId)
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