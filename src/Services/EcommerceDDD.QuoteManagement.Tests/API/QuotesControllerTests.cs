namespace EcommerceDDD.QuoteManagement.Tests;

public class QuotesControllerTests
{
	public QuotesControllerTests()
	{
		_quotesController = new QuotesController(_commandBus, _queryBus);
		_quotesInternalController = new QuotesInternalController(_commandBus, _queryBus);
	}

	[Fact]
	public async Task ListHistory_WithQuoteId_ShouldReturnListOfQuoteEventHistory()
	{
		// Given
		var quoteId = Guid.NewGuid();
		var expectedData = new List<QuoteEventHistory>()
		{
			new QuoteEventHistory(
				Guid.NewGuid(),
				quoteId,
				typeof(QuoteOpen).Name,
				"event data",
				DateTime.UtcNow
			),
			new QuoteEventHistory(
				Guid.NewGuid(),
				quoteId,
				typeof(QuoteItemAdded).Name,
				"event data",
				DateTime.UtcNow
			)
		};

		_queryBus.SendAsync(Arg.Any<GetQuoteEventHistory>(), CancellationToken.None)
			.Returns(Result.Ok<IReadOnlyList<QuoteEventHistory>>(expectedData));

		// When
		var response = await _quotesController.ListHistory(quoteId,
			CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		var data = Assert.IsAssignableFrom<IReadOnlyList<QuoteEventHistory>>(okResult.Value);
		Assert.Equal(expectedData.Count, data.Count);
	}

	[Fact]
	public async Task OpenCustomerQuote_WithOpenQuoteRequest_ShouldOpeQuoteForCustomer()
	{
		// Given
		_commandBus.SendAsync(Arg.Any<OpenQuote>(), CancellationToken.None)
			.Returns(Result.Ok());

		var request = new OpenQuoteRequest()
		{
			CurrencyCode = Currency.USDollar.Code
		};

		// When
		var response = await _quotesController
			.OpenQuoteForCustomer(request, CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	[Fact]
	public async Task AddItem_WithAddQuoteItemRequest_ShouldAddAnItemToQuote()
	{
		// Given
		Guid quoteId = Guid.NewGuid();

		_commandBus.SendAsync(Arg.Any<AddQuoteItem>(), CancellationToken.None)
			.Returns(Result.Ok());

		var request = new AddQuoteItemRequest()
		{
			ProductId = Guid.NewGuid(),
			Quantity = 10
		};

		// When
		var response = await _quotesController.AddItem(quoteId, request,
			CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	[Fact]
	public async Task RemoveItem_WithQuoteId_and_ProductId_ShouldRemoveItemFromQuote()
	{
		// Given
		Guid quoteId = Guid.NewGuid();
		Guid productId = Guid.NewGuid();

		_commandBus.SendAsync(Arg.Any<RemoveQuoteItem>(), CancellationToken.None)
			.Returns(Result.Ok());

		// When
		var response = await _quotesController.RemoveItem(quoteId, productId,
			CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	[Fact]
	public async Task CancelQuote_WithQuoteId_ShouldCancelQuote()
	{
		// Given
		Guid quoteId = Guid.NewGuid();

		_commandBus.SendAsync(Arg.Any<CancelQuote>(), CancellationToken.None)
			.Returns(Result.Ok());

		// When
		var response = await _quotesController.Cancel(quoteId,
			CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	#region INTERNAL
	[Fact]
	public async Task ConfirmQuote_WithQuoteId_and_CurrencyCode_ShouldConfirmQuote()
	{
		// Given
		Guid quoteId = Guid.NewGuid();

		_commandBus.SendAsync(Arg.Any<ConfirmQuote>(), CancellationToken.None)
			.Returns(Result.Ok());

		// When
		var response = await _quotesInternalController.Confirm(quoteId,
			CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}
	#endregion

	private ICommandBus _commandBus = Substitute.For<ICommandBus>();
	private IQueryBus _queryBus = Substitute.For<IQueryBus>();
	private QuotesController _quotesController;
	private QuotesInternalController _quotesInternalController;
}
