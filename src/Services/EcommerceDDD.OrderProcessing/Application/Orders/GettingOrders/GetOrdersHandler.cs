namespace EcommerceDDD.OrderProcessing.Application.Orders.GettingOrders;

public class GetOrdersHandler(
	IQuoteService quoteService,
	IQuerySession querySession,
	IUserInfoRequester userInfoRequester
) : IQueryHandler<GetOrders, IReadOnlyList<OrderViewModel>>
{
	private readonly IQuoteService _quoteService = quoteService
		?? throw new ArgumentNullException(nameof(quoteService));
	private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));
	private readonly IUserInfoRequester _userInfoRequester = userInfoRequester
		?? throw new ArgumentNullException(nameof(userInfoRequester));

	public async Task<Result<IReadOnlyList<OrderViewModel>>> HandleAsync(GetOrders query, CancellationToken cancellationToken)
	{
		var userInfo = await _userInfoRequester.RequestUserInfoAsync();

		var orders = await _querySession.Query<OrderDetails>()
			.Where(o => o.CustomerId == userInfo!.CustomerId)
			.ToListAsync(cancellationToken);

		if (orders == null || orders.Count == 0)
			return Result.Ok<IReadOnlyList<OrderViewModel>>(Array.Empty<OrderViewModel>());

		var viewModelResults = await Task.WhenAll(
			orders.Select(orderDetails => BuildOrderViewModelAsync(orderDetails, cancellationToken))
		);

		var failedResult = viewModelResults.FirstOrDefault(r => r.IsFailed);
		if (failedResult is not null)
			return Result.Fail<IReadOnlyList<OrderViewModel>>(failedResult.Errors);

		return Result.Ok<IReadOnlyList<OrderViewModel>>(viewModelResults.Select(r => r.Value!).ToList());
	}

	private async Task<Result<OrderViewModel>> BuildOrderViewModelAsync(OrderDetails order, CancellationToken cancellationToken)
	{
		List<OrderLineViewModel> orderLines;
		string currencySymbol;

		if (order.OrderStatus == OrderStatus.Placed)
		{
			var quoteResult = await GetQuoteAsync(order, cancellationToken);
			if (quoteResult.IsFailed)
				return Result.Fail<OrderViewModel>(quoteResult.Errors);

			var quote = quoteResult.Value!;

			if (!quote.Items!.Any())
				return Result.Fail<OrderViewModel>(
					new RecordNotFoundError($"No quote items found for customer."));

			orderLines = quote.Items!.Select(item =>
				new OrderLineViewModel
				{
					ProductId = item.ProductId!.Value,
					ProductName = item.ProductName,
					UnitPrice = Convert.ToDecimal(item.UnitPrice),
					Quantity = item.Quantity!.Value
				}).ToList();

			currencySymbol = Currency.OfCode(quote.CurrencyCode!).Symbol;
			order.TotalPrice = Convert.ToDecimal(quote.TotalPrice);
		}
		else
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

		return Result.Ok(new OrderViewModel
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
		});
	}

	private async Task<Result<QuoteViewModel>> GetQuoteAsync(OrderDetails orderDetails, CancellationToken cancellationToken)
	{
		try
		{
			var response = await _quoteService
				.GetQuoteDetailsAsync(orderDetails.QuoteId, cancellationToken);

			if (response is null)
				return Result.Fail<QuoteViewModel>(
					new RecordNotFoundError($"Quote data not found."));

			return Result.Ok(response);
		}
		catch (Exception)
		{
			return Result.Fail<QuoteViewModel>(
				$"An error occurred when getting quote {orderDetails.QuoteId} for order {orderDetails.Id}.");
		}
	}
}
