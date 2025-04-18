namespace EcommerceDDD.OrderProcessing.Application.Orders.GettingOrders;

public class GetOrdersHandler(
	IQuerySession querySession,
	IUserInfoRequester userInfoRequester,
	IIntegrationHttpService integrationHttpService,
	IConfiguration configuration
) : IQueryHandler<GetOrders, IList<OrderViewModel>>
{
	private readonly IQuerySession _querySession = querySession;
	private readonly IIntegrationHttpService _integrationHttpService = integrationHttpService;
	private readonly IConfiguration _configuration = configuration;
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<IList<OrderViewModel>> HandleAsync(GetOrders query, CancellationToken cancellationToken)
	{
		var userInfo = await _userInfoRequester.RequestUserInfoAsync();

		var orders = await _querySession.Query<OrderDetails>()
			.Where(o => o.CustomerId == userInfo!.CustomerId)
			.ToListAsync(cancellationToken);

		if (orders == null || orders.Count == 0)
			return Array.Empty<OrderViewModel>();

		var viewModels = await Task.WhenAll(
			orders.Select(BuildOrderViewModel)
		);

		return viewModels.ToList();
	}

	private async Task<OrderViewModel> BuildOrderViewModel(OrderDetails order)
	{
		List<OrderLineViewModel> orderLines;
		string currencySymbol;

		if (order.OrderStatus == OrderStatus.Placed) // order was just placed
		{
			// retrieving quote details
			var quote = await GetQuoteAsync(order);
			orderLines = quote.Items.Select(item => 
				new OrderLineViewModel
				{
					ProductId = item.ProductId,
					ProductName = item.ProductName,
					UnitPrice = item.UnitPrice,
					Quantity = item.Quantity
				}).ToList();

			currencySymbol = Currency
				.OfCode(quote.CurrencyCode).Symbol;
			order.TotalPrice = quote.TotalPrice;
		}
		else // Order has been processed and contains confirmed items
		{
			orderLines = order.OrderLines.Select(item => new OrderLineViewModel
			{
				ProductId = item.ProductId,
				ProductName = item.ProductName,
				UnitPrice = item.UnitPrice,
				Quantity = item.Quantity
			}).ToList();

			currencySymbol = order.Currency.Symbol;
		}

		return new OrderViewModel
		{
			OrderId = order.Id,
			QuoteId = order.QuoteId,
			CustomerId = order.CustomerId,
			CreatedAt = order.CreatedAt,
			StatusCode = (int)order.OrderStatus,
			StatusText = order.OrderStatus.ToString(),
			OrderLines = orderLines,
			CurrencySymbol = currencySymbol,
			TotalPrice = order.TotalPrice
		};
	}

	private async Task<QuoteViewModelResponse> GetQuoteAsync(OrderDetails order)
	{
		var apiRoute = _configuration["ApiRoutes:QuoteManagement"];
		var quoteResponse = await _integrationHttpService.GetAsync<QuoteViewModelResponse>(
			$"{apiRoute}/{order.QuoteId}/details");

		if (quoteResponse == null || !quoteResponse.Success)
			throw new ApplicationLogicException(quoteResponse?.Message
				?? $"Failed to retrieve quote for customer {order.CustomerId}.");

		return quoteResponse.Data!;
	}
}