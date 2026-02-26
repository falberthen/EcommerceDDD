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
		var orderNotificationService = Substitute.For<IOrderNotificationService>();
		var quoteService = Substitute.For<IQuoteService>();

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

		quoteService.GetQuoteDetailsAsync(_quoteId.Value, Arg.Any<CancellationToken>())
			.Returns(viewModelResponse);

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(orderNotificationService, quoteService, orderWriteRepository);

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
	public async Task PlaceOrder_WithEmptyQuoteItems_ShouldReturnFailure()
	{
		// Given
		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		var orderNotificationService = Substitute.For<IOrderNotificationService>();
		var quoteService = Substitute.For<IQuoteService>();

		quoteService.GetQuoteDetailsAsync(_quoteId.Value, Arg.Any<CancellationToken>())
			.Returns(new QuoteViewModel { QuoteId = _quoteId.Value, Items = new List<QuoteItemViewModel>() });

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(orderNotificationService, quoteService, orderWriteRepository);

		// When
		var result = await placeOrderHandler.HandleAsync(placeOrder, CancellationToken.None);

		// Then
		Assert.True(result.IsFailed);
	}

	private readonly QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
}
