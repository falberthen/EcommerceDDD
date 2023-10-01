namespace EcommerceDDD.Quotes.Tests.Application;

public class AddQuoteItemHandlerTests
{
    [Fact]
    public async Task AddQuoteItem_WithCommand_ShouldAddQuoteItem()
    {
        // Given        
        var response = new IntegrationHttpResponse<List<ProductViewModel>>()
        {
            Success = true,
            Data = new List<ProductViewModel>()
            {
                new ProductViewModel(
                    _productId.Value,
                    _productName,
                    _productQuantity,
                    _currency.Symbol
                )
            }
        };

        _integrationHttpService
            .FilterAsync<List<ProductViewModel>>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(Task.FromResult(response));

        var quote = Quote.Create(_customerId);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var addQuoteItem = AddQuoteItem.Create(quote.Id, _productId, _productQuantity, _currency);
        var addQuoteItemHandler = new AddQuoteItemHandler(_integrationHttpService, quoteWriteRepository);

        // When
        await addQuoteItemHandler.Handle(addQuoteItem, CancellationToken.None);

        // Then        
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task ChangeQuoteItemQuantity_WithCommand_ShouldChangeQuoteItemQuantity()
    {
        // Given
        var response = new IntegrationHttpResponse<List<ProductViewModel>>()
        {
            Success = true,
            Data = new List<ProductViewModel>()
            {
                new ProductViewModel(
                    _productId.Value,
                    _productName,
                    _productQuantity,
                    _currency.Symbol
                )
            }
        };
        _integrationHttpService
            .FilterAsync<List<ProductViewModel>>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(Task.FromResult(response));

        var quote = Quote.Create(_customerId);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var addQuoteItem = AddQuoteItem.Create(quote.Id, _productId, _productQuantity, _currency);
        var addQuoteItemHandler = new AddQuoteItemHandler(_integrationHttpService, quoteWriteRepository);
        await addQuoteItemHandler.Handle(addQuoteItem, CancellationToken.None);

        var productNewQuantity = 12;
        var changeQuoteItem = AddQuoteItem.Create(quote.Id, _productId, productNewQuantity, _currency);

        // When
        await addQuoteItemHandler.Handle(changeQuoteItem, CancellationToken.None);

        // Then   
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Items.Count.Should().Be(1);
        storedQuote.Items.First().Quantity.Should().Be(productNewQuantity);        
    }

    private const int _productQuantity = 1;
    private const string _productName = "Product XYZ";
    private Currency _currency = Currency.OfCode(Currency.USDollar.Code);
    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private IIntegrationHttpService _integrationHttpService = Substitute.For<IIntegrationHttpService>();
}