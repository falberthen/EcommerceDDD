using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.Quotes.Application.AddingQuoteItem;
using EcommerceDDD.IntegrationServices.Products;
using static EcommerceDDD.IntegrationServices.Products.ProductsResponse;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Quotes.Tests.Application;

public class AddQuoteItemCommandHandlerTests
{
    [Fact]
    public async Task AddQuoteItem_WithCommand_ShouldAddQuoteItem()
    {
        // Given        
        _checker.Setup(p => p.CanCustomerOpenNewQuote(It.IsAny<CustomerId>()))
            .Returns(Task.FromResult(true));

        _productService.Setup(p => p.GetProductByIds(It.IsAny<string>(),
            new Guid[] { _productId.Value }, _currencyCode))
            .Returns(Task.FromResult(
                new List<ProductViewModel>() 
                { 
                    new ProductViewModel(
                        _productId.Value, 
                        _productName,
                        _productQuantity,
                        _currencySymbol
                    ) 
                }
            ));

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var quote = await Quote.CreateNew(_customerId, _checker.Object);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var addQuoteItem = new AddQuoteItem(quote.Id, _productId, _productQuantity, _currencyCode);
        var addQuoteItemHandler = new AddQuoteItemHandler(_productService.Object, quoteWriteRepository, options.Object);

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
        _checker.Setup(p => p.CanCustomerOpenNewQuote(_customerId))
            .Returns(Task.FromResult(true));

        _productService.Setup(p => p.GetProductByIds(It.IsAny<string>(),
            new Guid[] { _productId.Value }, _currencyCode))
            .Returns(Task.FromResult(
                new List<ProductViewModel>()
                {
                    new ProductViewModel(
                        _productId.Value,
                        _productName,
                        _productQuantity,
                        _currencySymbol
                    )
                }
            ));

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var quote = await Quote.CreateNew(_customerId, _checker.Object);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var addQuoteItem = new AddQuoteItem(quote.Id, _productId, _productQuantity, _currencyCode);
        var addQuoteItemHandler = new AddQuoteItemHandler(_productService.Object, quoteWriteRepository, options.Object);
        await addQuoteItemHandler.Handle(addQuoteItem, CancellationToken.None);

        var productNewQuantity = 12;
        var changeQuoteItem = new AddQuoteItem(quote.Id, _productId, productNewQuantity, _currencyCode);

        // When
        await addQuoteItemHandler.Handle(changeQuoteItem, CancellationToken.None);

        // Then   
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Items.Count.Should().Be(1);
        storedQuote.Items.First().Quantity.Should().Be(productNewQuantity);        
    }

    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private const int _productQuantity = 1;
    private const string _productName = "Product XYZ";
    private string _currencyCode = Currency.USDollar.Code;
    private string _currencySymbol = Currency.USDollar.Symbol;
    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
    private Mock<IProductsService> _productService = new();
}