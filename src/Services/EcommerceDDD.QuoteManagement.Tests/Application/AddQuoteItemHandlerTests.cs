using NSubstitute;

namespace EcommerceDDD.QuoteManagement.Tests.Application;

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

        _productMapper
            .MapProductFromCatalogAsync(Arg.Any<IEnumerable<ProductId>>(), Arg.Any<Currency>())
            .Returns(new List<ProductViewModel>()
            {
                new ProductViewModel(_productId.Value, 
                    _productName, 10, Currency.USDollar.Code)
            });

        var quote = Quote.OpenQuote(_customerId, _currency);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var addQuoteItem = AddQuoteItem.Create(quote.Id, _productId, _productQuantity);
        var addQuoteItemHandler = new AddQuoteItemHandler(quoteWriteRepository, _productMapper);

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
        _productMapper
            .MapProductFromCatalogAsync(Arg.Any<IEnumerable<ProductId>>(), Arg.Any<Currency>())
            .Returns(new List<ProductViewModel>()
            {
                new ProductViewModel(_productId.Value, 
                    _productName, 10, Currency.USDollar.Code)
            });

        var quote = Quote.OpenQuote(_customerId, _currency);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var addQuoteItem = AddQuoteItem.Create(quote.Id, _productId, _productQuantity);
        var addQuoteItemHandler = new AddQuoteItemHandler(quoteWriteRepository, _productMapper);
        await addQuoteItemHandler.Handle(addQuoteItem, CancellationToken.None);

        var productNewQuantity = 12;
        var changeQuoteItem = AddQuoteItem.Create(quote.Id, _productId, productNewQuantity);

        // When
        await addQuoteItemHandler.Handle(changeQuoteItem, CancellationToken.None);

        // Then   
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Items.Count.Should().Be(1);
        storedQuote.Items.First().ProductItem.Quantity.Should().Be(productNewQuantity);
    }

    private const int _productQuantity = 1;
    private const string _productName = "Product XYZ";
    private Currency _currency = Currency.OfCode(Currency.USDollar.Code);
    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private IIntegrationHttpService _integrationHttpService = Substitute.For<IIntegrationHttpService>();
    private IProductMapper _productMapper = Substitute.For<IProductMapper>();

}