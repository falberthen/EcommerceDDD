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
		var placeOrderHandler = new PlaceOrderHandler(orderNotificationService, quoteService,
			orderWriteRepository, GivenCurrentCustomer(customerId.Value));

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
	public async Task PlaceOrder_WithQuoteFromAnotherCustomer_ShouldReturnForbidden()
	{
		// Given
		var quoteOwnerId = Guid.NewGuid();
		var currentCustomerId = Guid.NewGuid();

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		var orderNotificationService = Substitute.For<IOrderNotificationService>();
		var quoteService = Substitute.For<IQuoteService>();

		quoteService.GetQuoteDetailsAsync(_quoteId.Value, Arg.Any<CancellationToken>())
			.Returns(new QuoteViewModel
			{
				QuoteId = _quoteId.Value,
				CustomerId = quoteOwnerId,
				CurrencyCode = Currency.USDollar.Code,
				Items = new List<QuoteItemViewModel>()
				{
					new QuoteItemViewModel()
					{
						ProductId = Guid.NewGuid(),
						ProductName = "Product",
						Quantity = 1,
						UnitPrice = 200
					}
				}
			});

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(orderNotificationService, quoteService,
			orderWriteRepository, GivenCurrentCustomer(currentCustomerId));

		// When
		var result = await placeOrderHandler.HandleAsync(placeOrder, CancellationToken.None);

		// Then
		Assert.True(result.IsFailed);
		Assert.Contains(result.Errors, e => e is ForbiddenError);
		Assert.Empty(orderWriteRepository.AggregateStream);
	}

	[Fact]
	public async Task PlaceOrder_WithEmptyQuoteItems_ShouldReturnFailure()
	{
		// Given
		var customerId = Guid.NewGuid();
		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		var orderNotificationService = Substitute.For<IOrderNotificationService>();
		var quoteService = Substitute.For<IQuoteService>();

		quoteService.GetQuoteDetailsAsync(_quoteId.Value, Arg.Any<CancellationToken>())
			.Returns(new QuoteViewModel
			{
				QuoteId = _quoteId.Value,
				CustomerId = customerId,
				Items = new List<QuoteItemViewModel>()
			});

		var placeOrder = PlaceOrder.Create(_quoteId);
		var placeOrderHandler = new PlaceOrderHandler(orderNotificationService, quoteService,
			orderWriteRepository, GivenCurrentCustomer(customerId));

		// When
		var result = await placeOrderHandler.HandleAsync(placeOrder, CancellationToken.None);

		// Then
		Assert.True(result.IsFailed);
	}

	private static IUserInfoRequester GivenCurrentCustomer(Guid customerId)
	{
		var userInfoRequester = Substitute.For<IUserInfoRequester>();
		userInfoRequester.GetCurrentUser()
			.Returns(new UserInfo()
			{
				UserId = Guid.NewGuid().ToString(),
				CustomerId = customerId
			});

		return userInfoRequester;
	}

	private readonly QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
}
