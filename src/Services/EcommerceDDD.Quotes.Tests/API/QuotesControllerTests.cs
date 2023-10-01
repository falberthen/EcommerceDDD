namespace EcommerceDDD.Quotes.Tests;

public class QuotesControllerTests
{
    public QuotesControllerTests()
    {
        _quotesController = new QuotesController(
            _commandBus,_queryBus);
    }

    [Fact]
    public async Task GetById_WithQuoteId_ShouldReturnQuoteViewModel()
    {
        // Given
        var quoteId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var expectedData = new QuoteViewModel
        {
            CreatedAt = DateTime.UtcNow,
            CurrencyCode = Currency.USDollar.Code,
            CurrencySymbol = Currency.USDollar.Symbol,
            CustomerId = Guid.NewGuid(),
            Items = new List<QuoteItemViewModel> 
            { 
                new QuoteItemViewModel 
                {
                    CurrencySymbol = Currency.USDollar.Symbol,
                    ProductId = Guid.NewGuid(),
                    Quantity = 10,
                    ProductName = "Product X",
                    UnitPrice = 100m
                } 
            }
        };

        _queryBus.Send(Arg.Any<GetConfirmedQuoteById>())
            .Returns(expectedData);

        // When
        var response = await _quotesController.GetById(quoteId);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<QuoteViewModel>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task ListHistory_WithQuoteId_ShouldReturnListOfQuoteEventHistory()
    {
        // Given
        var quoteId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var expectedData = new List<QuoteEventHistory>()
        {
            new QuoteEventHistory(
                Guid.NewGuid(),
                quoteId,
                typeof(QuoteOpen).Name,
                "event data"
            ),
            new QuoteEventHistory(
                Guid.NewGuid(),
                quoteId,
                typeof(QuoteItemAdded).Name,
                "event data"
            )
        };

        _queryBus.Send(Arg.Any<GetQuoteEventHistory>())
            .Returns(expectedData);

        // When
        var response = await _quotesController.ListHistory(quoteId);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<QuoteEventHistory>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task OpenCustomerQuote_WithOpenQuoteRequest_ShouldOpeQuoteForCustomer()
    {
        // Given
        Guid customerId = Guid.NewGuid();

        await _commandBus.Send(Arg.Any<OpenQuote>());

        var request = new OpenQuoteRequest()
        {
            CustomerId = customerId
        };

        // When
        var response = await _quotesController.OpenCustomerQuote(request);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddItem_WithAddQuoteItemRequest_ShouldAddAnItemToQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();

        await _commandBus.Send(Arg.Any<AddQuoteItem>());

        var request = new AddQuoteItemRequest()
        {
            CurrencyCode = Currency.USDollar.Code,
            ProductId = Guid.NewGuid(),
            Quantity = 10
        };

        // When
        var response = await _quotesController.AddItem(quoteId, request);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RemoveItem_WithQuoteId_and_ProductId_ShouldRemoveItemFromQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await _commandBus.Send(Arg.Any<RemoveQuoteItem>());

        // When
        var response = await _quotesController.RemoveItem(quoteId, productId);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CancelQuote_WithQuoteId_ShouldCancelQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();

        await _commandBus.Send(Arg.Any<CancelQuote>());

        // When
        var response = await _quotesController.Cancel(quoteId);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ConfirmQuote_WithQuoteId_and_CurrencyCode_ShouldConfirmQuote()
    {
        // Given
        Guid quoteId = Guid.NewGuid();
        var currencyCode = Currency.USDollar.Code;

        await _commandBus.Send(Arg.Any<ConfirmQuote>());

        // When
        var response = await _quotesController.Confirm(quoteId, currencyCode);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private QuotesController _quotesController;
}