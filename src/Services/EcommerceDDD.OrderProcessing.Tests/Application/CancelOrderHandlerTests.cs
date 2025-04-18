namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class CancelOrderHandlerTests
{
    [Fact]
    public async Task CancelOrder_WithCommand_ShouldCancelOrder()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        
        var orderData = new OrderData(customerId, quoteId);
        var order = Order.Place(orderData);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        await orderWriteRepository
            .AppendEventsAsync(order);

        var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.CanceledByCustomer);
        var cancelOrderHandler = new CancelOrderHandler(_orderStatusBroadcaster, orderWriteRepository);

        // When
        await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

        // Then
        var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(canceledOrder);
		Assert.Equal(canceledOrder.CustomerId, customerId);
		Assert.Equal(canceledOrder.QuoteId, quoteId);
		Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
    }

    private IOrderStatusBroadcaster _orderStatusBroadcaster = Substitute.For<IOrderStatusBroadcaster>();
}