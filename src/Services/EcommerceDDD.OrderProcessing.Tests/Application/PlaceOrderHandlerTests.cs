using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class PlaceOrderHandlerTests
{
	[Fact]
	public async Task PlaceOrder_WithCommand_ShouldPlaceOrder()
	{
		// Given
		var productId = ProductId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());
		var currency = Currency.OfCode(Currency.USDollar.Code);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		var apiClient = new ApiGatewayClient(_requestAdapter);

		// return mocked view model
		var viewModelResponse = new QuoteViewModel()
		{
			QuoteId = _quoteId.Value,
			CustomerId = customerId.Value,
			CurrencySymbol = currency.Symbol,
			CurrencyCode = currency.Code,
			Items = new List<QuoteItemViewModel>()
			{
				new QuoteItemViewModel()
				{
					ProductId = productId.Value,
					ProductName = "Product",
					Quantity = 10,
					UnitPrice = 200
				}
			}
		};

		var quoteApiResponse = new QuoteViewModelApiResponse()
		{
			Data = viewModelResponse,
			Success = true
		};

		// mocked kiota request
		_requestAdapter.SendAsync(
			Arg.Is<RequestInformation>(req => req.PathParameters.Values.Contains(_quoteId.Value)),
			Arg.Any<ParsableFactory<QuoteViewModelApiResponse>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>())
		.Returns(quoteApiResponse);

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(apiClient, orderWriteRepository, _orderStatusBroadcaster);

		// When
		await placeOrderHandler.HandleAsync(placeOrder, CancellationToken.None);

		// Then
		var placedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(placedOrder);
		Assert.Equal(placedOrder.CustomerId, customerId);
		Assert.Equal(placedOrder.QuoteId, _quoteId);
		Assert.Equal(OrderStatus.Placed, placedOrder.Status);
	}

	[Fact]
	public async Task PlaceOrder_WithEmptyQuoteItems_ShouldThrowException()
	{
		// Given
		var currency = Currency.OfCode(Currency.USDollar.Code);
		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		var apiClient = new ApiGatewayClient(_requestAdapter);

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(apiClient, orderWriteRepository, _orderStatusBroadcaster);

		// When & Then
		await Assert.ThrowsAsync<ApplicationLogicException>(() =>
			placeOrderHandler.HandleAsync(placeOrder, CancellationToken.None));
	}

	private readonly QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());	
	private IOrderStatusBroadcaster _orderStatusBroadcaster = Substitute.For<IOrderStatusBroadcaster>();
	private IRequestAdapter _requestAdapter = Substitute.For<IRequestAdapter>();
}