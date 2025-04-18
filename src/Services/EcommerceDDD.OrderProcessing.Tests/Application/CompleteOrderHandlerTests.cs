namespace EcommerceDDD.OrderProcessing.Tests.Application;

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
        var orderData = new OrderData(customerId, quoteId, currency, quoteItems);
        var order = Order.Place(orderData);
        
        order.Process(orderData);
        order.RecordPayment(paymentId, totalPaid);
        order.RecordShipment(shipmentId);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        await orderWriteRepository
            .AppendEventsAsync(order);

        var completeOrder = CompleteOrder.Create(order.Id, shipmentId);
        var completeOrderHandler = new CompleteOrderHandler(_orderStatusBroadcaster, orderWriteRepository);

        // When
        await completeOrderHandler.HandleAsync(completeOrder, CancellationToken.None);

        // Then
        var completedOrder = orderWriteRepository.AggregateStream.First().Aggregate;


		Assert.NotNull(completedOrder);
		Assert.Equal(completedOrder.CustomerId, customerId);
		Assert.Equal(completedOrder.QuoteId, quoteId);
		Assert.Equal(completedOrder.PaymentId, paymentId);
		Assert.Equal(completedOrder.ShipmentId, shipmentId);
		Assert.Equal(completedOrder.OrderLines.Count, quoteItems.Count);
		Assert.Equal(OrderStatus.Completed, completedOrder.Status);
    }

    private IOrderStatusBroadcaster _orderStatusBroadcaster = Substitute.For<IOrderStatusBroadcaster>();
}