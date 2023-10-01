namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;
    private readonly IProductItemsMapper _productItemsMapper;

    public PlaceOrderHandler(
        IIntegrationHttpService integrationHttpService,
        IEventStoreRepository<Order> orderWriteRepository,
        IProductItemsMapper productItemsMapper)
    {
        _integrationHttpService= integrationHttpService;
        _orderWriteRepository = orderWriteRepository;
        _productItemsMapper = productItemsMapper;
    }

    public async Task Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        // Getting quote data
        var quote = await GetConfirmedQuoteById(command.QuoteId);

        // Getting product data from catalog
        var currency = Currency.OfCode(quote.CurrencyCode);
        var productItemsData = await _productItemsMapper
            .MatchProductsFromCatalog(quote.Items, currency);

        var orderData = new OrderData(
            QuoteId.Of(quote.QuoteId),
            CustomerId.Of(quote.CustomerId),
            productItemsData,
            currency);

        var order = Order.Create(orderData);

        await _orderWriteRepository
            .AppendEventsAsync(order);
    }

    private async Task<QuoteViewModelResponse> GetConfirmedQuoteById(QuoteId quoteId)
    {
        var response = await _integrationHttpService.GetAsync<QuoteViewModelResponse>(
            $"api/quotes/{quoteId.Value}");

        if (response is null)
            throw new ApplicationLogicException($"An error occurred retrieving quote {quoteId}.");

        if (!response.Success)
            throw new ApplicationLogicException(response.Message);

        var responseData = response.Data!;
        return responseData;
    }    
}

public record QuoteViewModelResponse(
    Guid QuoteId,
    Guid CustomerId,
    List<QuoteItemViewModel> Items,
    string CurrencyCode);

public record class QuoteItemViewModel(
    Guid ProductId,
    int Quantity);