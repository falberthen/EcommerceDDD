namespace EcommerceDDD.OrderProcessing.Application.Orders.GettingOrders;

public class GetOrdersHandler : IQueryHandler<GetOrders, IList<OrderViewModel>>
{
    private readonly IQuerySession _querySession;
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IConfiguration _configuration;

    public GetOrdersHandler(
        IQuerySession querySession, 
        IIntegrationHttpService integrationHttpService, 
        IConfiguration configuration)
    {
        _querySession = querySession;
        _integrationHttpService = integrationHttpService;
        _configuration = configuration;
    }

    public async Task<IList<OrderViewModel>> Handle(GetOrders query, CancellationToken cancellationToken)
    {
        var orders = await _querySession.Query<OrderDetails>()
            .Where(q => q.CustomerId == query.CustomerId.Value)
            .ToListAsync();

        if (orders is null)
            return Array.Empty<OrderViewModel>();

        var viewModels = await Task.WhenAll(orders.Select(async order =>
        {
            var quote = order.OrderStatus == OrderStatus.Placed 
                ? await GetQuote(order) : null;

            var orderLines = order.OrderStatus == OrderStatus.Placed
                ? quote!.Items.Select(quoteItem =>
                    new OrderLineViewModel
                    {
                        ProductId = quoteItem.ProductId,
                        ProductName = quoteItem.ProductName,
                        UnitPrice = quoteItem.UnitPrice,
                        Quantity = quoteItem.Quantity
                    }).ToList()
                : order.OrderLines.Select(orderline =>
                    new OrderLineViewModel
                    {
                        ProductId = orderline.ProductId,
                        ProductName = orderline.ProductName,
                        UnitPrice = orderline.UnitPrice,
                        Quantity = orderline.Quantity
                    }).ToList();

            string currencySymbol = quote is not null ?
                Currency.OfCode(quote?.CurrencyCode!).Symbol : order!.Currency.Symbol;

            var viewModel = new OrderViewModel
            {
                OrderId = order.Id,
                QuoteId = order.QuoteId,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                StatusCode = (int)order.OrderStatus,
                StatusText = order.OrderStatus.ToString(),
                OrderLines = orderLines,
                CurrencySymbol = currencySymbol,
                TotalPrice = quote?.TotalPrice ?? order.TotalPrice
            };

            return viewModel;
        }));

        return viewModels.ToList();
    }


    private async Task<QuoteViewModelResponse> GetQuote(OrderDetails order)
    {
        var apiRoute = _configuration["ApiRoutes:QuoteManagement"];
        var response = await _integrationHttpService.GetAsync<QuoteViewModelResponse>(
            $"{apiRoute}/{order.CustomerId}/quote/{order.QuoteId}")
            ?? throw new ApplicationLogicException(
                $"An error occurred retrieving quote for customer {order.CustomerId}.");

        if (!response.Success)
            throw new ApplicationLogicException(response?.Message ?? string.Empty);

        var responseData = response.Data!;
        return responseData;
    }
}