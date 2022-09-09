using EcommerceDDD.Core.Testing;
using EcommerceDDD.Quotes.Domain;
using EcommerceDDD.IntegrationServices.Base;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.Quotes.Application.AddingQuoteItem;
using EcommerceDDD.Quotes.Application.PlacingOrderFromQuote;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using static EcommerceDDD.IntegrationServices.Products.Responses.ProductsResponse;

namespace EcommerceDDD.Quotes.Tests.Application;

public class PlaceOrderFromQuoteCommandHandlerTests
{
    [Fact]
    public async Task AddQuoteItem_WithCommand_ShouldAddQuoteItem()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        const string productName = "Product XYZ";
        const int productQuantity = 1;
        var _currencyCode = Currency.USDollar.Code;
        var _currencySymbol = Currency.USDollar.Symbol;

        _checker.Setup(p => p.CanCustomerOpenNewQuote(customerId))
            .Returns(Task.FromResult(true));

        _productService.Setup(p => p.GetProductByIds(It.IsAny<string>(),
            new Guid[] { productId.Value }, _currencyCode))
            .Returns(Task.FromResult(
                new List<ProductViewModel>()
                {
                    new ProductViewModel(
                        productId.Value,
                        productName,
                        productQuantity,
                        _currencySymbol)
                }
            ));

        var quote = await Quote.CreateNew(customerId, _checker.Object);

        var quoteWriteRepository = new DummyEventStoreRepository<Quote>();
        await quoteWriteRepository.AppendEventsAsync(quote);

        var quoteItemRequest = quote.Items.Select(q =>
            new QuoteItemRequest(q.Id, q.ProductId.Value, q.Quantity))
            .ToList();

        var ordersService = new Mock<IOrdersService>();

        ordersService.Setup(p =>
            p.RequestPlaceOrder(It.IsAny<string>(), It.IsAny<PlaceOrderRequest>()))
            .Returns(Task.FromResult(
                new IntegrationServiceResponse()
                {
                    Success = true
                }
            ));

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var addQuoteItem = new AddQuoteItem(quote.Id, productId, productQuantity, _currencyCode);
        var addQuoteItemHandler = new AddQuoteItemHandler(_productService.Object, quoteWriteRepository, options.Object);
        await addQuoteItemHandler.Handle(addQuoteItem, CancellationToken.None);

        var placeOrderFromQuote = new PlaceOrderFromQuote(quote.Id, _currencyCode);
        var placeOrderFromQuoteHandler = new PlaceOrderFromQuoteHandler(ordersService.Object, quoteWriteRepository, options.Object);

        // When
        await placeOrderFromQuoteHandler.Handle(placeOrderFromQuote, CancellationToken.None);

        // Then
        var storedQuote = quoteWriteRepository.AggregateStream.First().Aggregate;
        storedQuote.Status.Should().Be(QuoteStatus.Confirmed);
    }

    private Mock<ICustomerQuoteOpennessChecker> _checker = new();
    private Mock<IProductsService> _productService = new();
}