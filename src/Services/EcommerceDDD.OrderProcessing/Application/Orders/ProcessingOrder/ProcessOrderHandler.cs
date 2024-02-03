namespace EcommerceDDD.OrderProcessing.Application.Orders.PlacingOrder;

public class ProcessOrderHandler : ICommandHandler<ProcessOrder>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IConfiguration _configuration;

    public ProcessOrderHandler(
        IIntegrationHttpService integrationHttpService,
        IEventStoreRepository<Order> orderWriteRepository,
        IEventPublisher eventPublisher,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _orderWriteRepository = orderWriteRepository;
        _eventPublisher = eventPublisher;
        _configuration = configuration;
    }

    public async Task Handle(ProcessOrder command, CancellationToken cancellationToken)
    {
        // Getting open quote data
        var quote = await GetQuote(command)
            ?? throw new RecordNotFoundException($"No open quote found for customer {command.CustomerId}.");
        var quoteId = QuoteId.Of(quote.QuoteId);

        // Building Order data
        var quoteItems = quote.Items.Select(qi =>
            new ProductItemData()
            {
                ProductId = ProductId.Of(qi.ProductId),
                Quantity = qi.Quantity,
                ProductName = qi.ProductName,
                UnitPrice = Money.Of(qi.UnitPrice, quote.CurrencyCode)
            }).ToList();

        var orderData = new OrderData(            
            CustomerId.Of(quote.CustomerId),
            quoteId,            
            Currency.OfCode(quote.CurrencyCode),
            quoteItems);

        // Processing order
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Order {command.OrderId} not found.");

        order.Process(orderData);

        // Keeping event for publishing
        var orderProcessedEvent = order.GetUncommittedEvents()
           .OfType<OrderProcessed>()
           .FirstOrDefault();

        // Persisting domain event
        await _orderWriteRepository
            .AppendEventsAsync(order);

        // publishing event
        await _eventPublisher
            .PublishEventAsync(orderProcessedEvent!, cancellationToken);
    }

    private async Task<QuoteViewModelResponse> GetQuote(ProcessOrder command)
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
}