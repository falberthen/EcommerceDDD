namespace EcommerceDDD.Orders.Tests.Application;

public class RecordPaymentToOrderHandlerTests
{
    [Fact]
    public async Task RecordPaymentToOrder_WithCommand_ShouldRecordPaidOrder()
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
        };

        var orderData = new OrderData(quoteId, customerId, quoteItems, currency);
        var totalPaid = Money.Of(100, currency.Code);
        var paymentId = PaymentId.Of(Guid.NewGuid());

        var order = Order.Create(orderData);
        
        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        await orderWriteRepository
            .AppendEventsAsync(order, CancellationToken.None);

        var recordPaymentToOrder = RecordPayment.Create(order.Id, paymentId, totalPaid);
        var recordPaymentToOrderHandler = new RecordPaymentHandler(
            _integrationHttpService.Object,
            _orderStatusBroadcaster.Object,
            orderWriteRepository);

        // When
        await recordPaymentToOrderHandler.Handle(recordPaymentToOrder, CancellationToken.None);

        // Then
        var paidOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        paidOrder.Id.Should().Be(order.Id);
        paidOrder.PaymentId.Should().Be(paymentId);
        paidOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        paidOrder.Status.Should().Be(OrderStatus.Paid);
    }

    private Mock<IIntegrationHttpService> _integrationHttpService = new();
    private Mock<IOrderStatusBroadcaster> _orderStatusBroadcaster = new();
}