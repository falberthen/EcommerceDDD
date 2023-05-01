namespace EcommerceDDD.Orders.Tests.Application;

public class CompleteOrderHandlerTests
{
    [Fact]
    public async Task CompleteOrder_WithCommand_ShouldCompleteOrder()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        var productName = "Product XYZ";
        var productPrice = Money.Of(10, Currency.USDollar.Code);
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var shipmentId = ShipmentId.Of(Guid.NewGuid());
        var paymentId = PaymentId.Of(Guid.NewGuid());

        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        }.ToList();

        var totalPaid = Money.Of(quoteItems.Sum(p => p.UnitPrice.Amount), currency.Code);
        var orderData = new OrderData(quoteId, customerId, quoteItems, currency);
        var order = Order.Create(orderData);
        order.RecordPayment(paymentId, totalPaid);
        order.RecordShippedEvent();

        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        await orderWriteRepository
            .AppendEventsAsync(order);

        var completeOrder = CompleteOrder.Create(order.Id, shipmentId);
        var completeOrderHandler = new CompleteOrderHandler(_orderStatusBroadcaster.Object, orderWriteRepository);

        // When
        await completeOrderHandler.Handle(completeOrder, CancellationToken.None);

        // Then
        var completedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        completedOrder.CustomerId.Should().Be(customerId);
        completedOrder.QuoteId.Should().Be(quoteId);
        completedOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        completedOrder.Status.Should().Be(OrderStatus.Completed);
    }

    private Mock<IOrderStatusBroadcaster> _orderStatusBroadcaster = new();
}