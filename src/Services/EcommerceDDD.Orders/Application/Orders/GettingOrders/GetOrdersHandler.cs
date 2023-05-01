namespace EcommerceDDD.Orders.Application.Orders.GettingOrders;

public class GetOrdersHandler : IQueryHandler<GetOrders, List<OrderViewModel>>
{
    private readonly IQuerySession _querySession;

    public GetOrdersHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public Task<List<OrderViewModel>> Handle(GetOrders query, CancellationToken cancellationToken)
    {
        var orders = _querySession.Query<OrderDetails>()
            .Where(q => q.CustomerId == query.CustomerId.Value)
            .ToList();

        List<OrderViewModel> viewModels = default!;

        if (orders != null)
        {
            viewModels = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                var viewModel = new OrderViewModel() with
                {
                    OrderId = order.Id,
                    QuoteId = order.QuoteId,
                    CustomerId = order.CustomerId,
                    CreatedAt = order.CreatedAt,
                    StatusCode = (int)order.OrderStatus,
                    StatusText = order.OrderStatus.ToString(),
                };

                var orderLines = new List<OrderLineViewModel>();
                foreach (var orderline in order.OrderLines)
                {
                    orderLines.Add(new OrderLineViewModel()
                    {
                        ProductId = orderline.ProductId,
                        ProductName = orderline.ProductName,
                        UnitPrice = orderline.UnitPrice,
                        Quantity = orderline.Quantity,
                    });
                }

                viewModel.OrderLines = orderLines;
                viewModel.CurrencySymbol = order.Currency.Symbol;
                viewModel.TotalPrice = order.TotalPrice;
                viewModels.Add(viewModel);
            }
        }

        return Task.FromResult(viewModels);
    }
}