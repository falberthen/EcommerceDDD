namespace EcommerceDDD.QuoteManagement.Tests;

public class QuotesControllerTests
{
    public QuotesControllerTests()
    {
        _quotesController = new QuotesController(
            _commandBus, _queryBus);
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
            .Returns(expectedData);

        // When
        var response = await _quotesController.ListHistory(quoteId,
            CancellationToken.None);

		// Then
		Assert.IsType<OkObjectResult>(response);
		var okResult = (OkObjectResult)response;

		Assert.IsType<ApiResponse<IList<QuoteEventHistory>>>(okResult.Value);
		var apiResponse = (ApiResponse<IList<QuoteEventHistory>>)okResult.Value;

		Assert.Equal(expectedData.Count, apiResponse.Data!.Count);
	}

    [Fact]
    public async Task OpenCustomerQuote_WithOpenQuoteRequest_ShouldOpeQuoteForCustomer()
    {
        // Given
        await _commandBus.SendAsync(Arg.Any<OpenQuote>(), CancellationToken.None);

        var request = new OpenQuoteRequest()
        {
            CurrencyCode = Currency.USDollar.Code
        };

        // When
        var response = await _quotesController
			.OpenQuoteForCustomer(request, CancellationToken.None);

		// Then
		Assert.IsType<OkObjectResult>(response);
	}

    [Fact]
    public async Task AddItem_WithAddQuoteItemRequest_ShouldAddAnItemToQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();

        await _commandBus.SendAsync(Arg.Any<AddQuoteItem>(), CancellationToken.None);

        var request = new AddQuoteItemRequest()
        {            
            ProductId = Guid.NewGuid(),
            Quantity = 10
        };

        // When
        var response = await _quotesController.AddItem(quoteId, request,
            CancellationToken.None);

        // Then
		Assert.IsType<OkObjectResult>(response);
	}

    [Fact]
    public async Task RemoveItem_WithQuoteId_and_ProductId_ShouldRemoveItemFromQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await _commandBus.SendAsync(Arg.Any<RemoveQuoteItem>(), CancellationToken.None);

        // When
        var response = await _quotesController.RemoveItem(quoteId, productId,
            CancellationToken.None);

		// Then
		Assert.IsType<OkObjectResult>(response);
	}

    [Fact]
    public async Task CancelQuote_WithQuoteId_ShouldCancelQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();

        await _commandBus.SendAsync(Arg.Any<CancelQuote>(), CancellationToken.None);

        // When
        var response = await _quotesController.Cancel(quoteId,
            CancellationToken.None);

		// Then
		Assert.IsType<OkObjectResult>(response);
	}

    [Fact]
    public async Task ConfirmQuote_WithQuoteId_and_CurrencyCode_ShouldConfirmQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();

        await _commandBus.SendAsync(Arg.Any<ConfirmQuote>(), CancellationToken.None);

        // When
        var response = await _quotesController.Confirm(quoteId,
            CancellationToken.None);

		// Then
		Assert.IsType<OkObjectResult>(response);
	}

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private QuotesController _quotesController;
}