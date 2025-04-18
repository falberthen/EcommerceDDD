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

		var responseConfirmedQuote = new IntegrationHttpResponse<QuoteViewModelResponse>()
		{
			Success = true,
			Data = new QuoteViewModelResponse(
				_quoteId.Value,
				customerId.Value,
				new List<QuoteItemViewModel>()
				{
					new QuoteItemViewModel(productId.Value, "Product", 10, 200)
				}, currency.Code, 200)
		};

		_integrationHttpService.GetAsync<QuoteViewModelResponse>(Arg.Any<string>())
			.Returns(Task.FromResult(responseConfirmedQuote));
		_integrationHttpService.PutAsync(Arg.Any<string>())
			.Returns(Task.FromResult(new IntegrationHttpResponse() { Success = true }));

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(
			_integrationHttpService, orderWriteRepository, _orderStatusBroadcaster, _configuration);

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
		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(_integrationHttpService,
			orderWriteRepository, _orderStatusBroadcaster, _configuration);

		// When & Then
		await Assert.ThrowsAsync<ApplicationLogicException>(() =>
			placeOrderHandler.HandleAsync(placeOrder, CancellationToken.None));
	}

	private readonly QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
	private IIntegrationHttpService _integrationHttpService = Substitute.For<IIntegrationHttpService>();
	private IOrderStatusBroadcaster _orderStatusBroadcaster = Substitute.For<IOrderStatusBroadcaster>();
	private IConfiguration _configuration = Substitute.For<IConfiguration>();
}