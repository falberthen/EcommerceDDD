using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Application.Orders.GettingOrders;

public class GetOrdersHandler(
	ApiGatewayClient apiGatewayClient,
	IQuerySession querySession,
	IUserInfoRequester userInfoRequester
) : IQueryHandler<GetOrders, IList<OrderViewModel>>
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;
	private readonly IQuerySession _querySession = querySession;	
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
				orders.Select(orderDetails => 
					BuildOrderViewModel(orderDetails, cancellationToken)
			)
		);

		return viewModels.ToList();
	}

	private async Task<OrderViewModel> BuildOrderViewModel(OrderDetails order, CancellationToken cancellationToken)
	{
		List<OrderLineViewModel> orderLines;
		string currencySymbol;

		if (order.OrderStatus == OrderStatus.Placed) // order was just placed
		{
			// retrieving quote details
			var quote = await GetQuoteAsync(order, cancellationToken);

			if (!quote.Items!.Any())
				throw new RecordNotFoundException($"No quote items found for customer.");

			orderLines = quote.Items!.Select(item => 
				new OrderLineViewModel
				{
					ProductId = item.ProductId!.Value,
					ProductName = item.ProductName,
					UnitPrice = Convert.ToDecimal(item.UnitPrice),
					Quantity = item.Quantity!.Value
				}).ToList();

			currencySymbol = Currency
				.OfCode(quote.CurrencyCode!).Symbol;
			order.TotalPrice = Convert.ToDecimal(quote.TotalPrice);
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

	private async Task<QuoteViewModel> GetQuoteAsync(OrderDetails orderDetails, CancellationToken cancellationToken)
	{
		try
		{
			var quoteRequestBuilder = _apiGatewayClient.Api.V2.Quotes[orderDetails.QuoteId];
			var response = await quoteRequestBuilder.Details
				.GetAsync(cancellationToken: cancellationToken);

			if (response?.Data is null)
				throw new ApplicationLogicException(response?.Message ?? string.Empty);

			return response.Data;
		}
		catch (Microsoft.Kiota.Abstractions.ApiException ex)
		{
			throw new ApplicationLogicException(
				$"An error occurred when getting quote {orderDetails.QuoteId} for order {orderDetails.Id}.", ex);
		}
	}
}