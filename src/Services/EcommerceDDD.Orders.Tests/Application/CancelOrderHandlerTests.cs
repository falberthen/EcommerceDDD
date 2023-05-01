namespace EcommerceDDD.Orders.Tests.Application;

public class CancelOrderHandlerTests
{
    [Fact]
    public async Task CancelOrder_WithCommand_ShouldCancelOrder()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        var productName = "Product XYZ";
        var productPrice = Money.Of(10, Currency.USDollar.Code);
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);

        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        }.ToList();

        var orderData = new OrderData(quoteId, customerId, quoteItems, currency);
        var order = Order.Create(orderData);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        await orderWriteRepository
            .AppendEventsAsync(order);

        var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.CanceledByUser);
        var cancelOrderHandler = new CancelOrderHandler(_orderStatusBroadcaster.Object, orderWriteRepository);

        // When
        await cancelOrderHandler.Handle(cancelOrder, CancellationToken.None);

        // Then
        var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        canceledOrder.CustomerId.Should().Be(customerId);
        canceledOrder.QuoteId.Should().Be(quoteId);
        canceledOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        canceledOrder.Status.Should().Be(OrderStatus.Canceled);
    }

    private Mock<IOrderStatusBroadcaster> _orderStatusBroadcaster = new();    
}