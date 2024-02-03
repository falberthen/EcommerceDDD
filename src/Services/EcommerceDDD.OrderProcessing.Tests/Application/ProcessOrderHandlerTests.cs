using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class ProcessOrderHandlerTests
{
    [Fact]
    public async Task PlaceOrder_WithCommand_ShouldPlaceOrder()
    {
        // Given
        var productId = ProductId.Of(Guid.NewGuid());
        var productName = "Product XYZ";
        var productPrice = Money.Of(10, Currency.USDollar.Code);
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var quoteId = QuoteId.Of(Guid.NewGuid());

        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var orderData = new OrderData(customerId, quoteId, currency, quoteItems);
        var order = Order.Place(orderData);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();

        var responseConfirmedQuote = new IntegrationHttpResponse<QuoteViewModelResponse>()
        {
            Success = true,
            Data = new QuoteViewModelResponse(
                quoteId.Value,
                customerId.Value,
                new List<QuoteItemViewModel>()
                {
                    new QuoteItemViewModel(productId.Value, "Product", 10, 200)
                }, currency.Code, 200)
        };

        _integrationHttpService.GetAsync<QuoteViewModelResponse>(Arg.Any<string>())
            .Returns(Task.FromResult(responseConfirmedQuote));
        _integrationHttpService.PutAsync(Arg.Any<string>())
            .Returns(Task.FromResult(new IntegrationHttpResponse() { Success = true }));

        await orderWriteRepository
            .AppendEventsAsync(order);

        var processOrder = ProcessOrder.Create(customerId, order.Id, quoteId);
        var processOrderHandler = new ProcessOrderHandler(
            _integrationHttpService, orderWriteRepository, _eventPublisher, _configuration);

        // When
        await processOrderHandler.Handle(processOrder, CancellationToken.None);

        // Then
        var placedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        placedOrder.CustomerId.Should().Be(customerId);
        placedOrder.QuoteId.Should().Be(quoteId);
        placedOrder.Status.Should().Be(OrderStatus.Processed);
    }

    private IIntegrationHttpService _integrationHttpService = Substitute.For<IIntegrationHttpService>();
    private IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private IConfiguration _configuration = Substitute.For<IConfiguration>();
}