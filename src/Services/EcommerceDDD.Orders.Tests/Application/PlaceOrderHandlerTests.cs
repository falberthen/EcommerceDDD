using static EcommerceDDD.Orders.Application.Orders.PlacingOrder.PlaceOrderHandler;

namespace EcommerceDDD.Orders.Tests.Application;

public class PlaceOrderHandlerTests
{
    [Fact]
    public async Task PlaceOrder_WithCommand_ShouldPlaceOrder()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        var productName = "Product XYZ";
        var productPrice = Money.Of(10, Currency.USDollar.Code);
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();

        var responseConfirmedQuote = new IntegrationHttpResponse<QuoteViewModelResponse>()
        {
            Success = true,
            Data = new QuoteViewModelResponse(
                quoteId.Value,
                customerId.Value,
                new List<QuoteItemViewModel>()
                {
                    new QuoteItemViewModel(productId.Value, 10)
                }, currency.Code)
        };

        _integrationHttpService.Setup(q =>
            q.GetAsync<QuoteViewModelResponse>(It.IsAny<string>()))
            .Returns(Task.FromResult(responseConfirmedQuote));

        var productItemsData = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        _productItemsMapper.Setup(p => p.MatchProductsFromCatalog(responseConfirmedQuote.Data.Items, currency))
            .Returns(Task.FromResult(productItemsData));

        var placeOrder = PlaceOrder.Create(quoteId);
        var placeOrderHandler = new PlaceOrderHandler(
            _integrationHttpService.Object, 
            orderWriteRepository, 
            _productItemsMapper.Object);

        // When
        await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

        // Then
        var placedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        placedOrder.CustomerId.Should().Be(customerId);
        placedOrder.QuoteId.Should().Be(quoteId);
        placedOrder.OrderLines.Count.Should().Be(productItemsData.Count);
        placedOrder.Status.Should().Be(OrderStatus.Placed);
    }

    [Fact]
    public async Task PlaceOrder_WithEmptyQuoteItems_ShouldThrowException()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var orderWriteRepository = new DummyEventStoreRepository<Order>();

        var placeOrder = PlaceOrder.Create(quoteId);
        var placeOrderHandler = new PlaceOrderHandler(_integrationHttpService.Object, orderWriteRepository, _productItemsMapper.Object);

        // When
        Func<Task> action = async () =>
            await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

        // Then
        await action.Should().ThrowAsync<ApplicationLogicException>();
    }

    private Mock<IProductItemsMapper> _productItemsMapper = new();
    private Mock<IIntegrationHttpService> _integrationHttpService = new();
}