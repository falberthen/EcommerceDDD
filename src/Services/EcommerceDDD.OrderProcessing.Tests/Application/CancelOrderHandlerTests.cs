namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class CancelOrderHandlerTests
{
	[Fact]
	public async Task CancelOrder_WhenNotYetProcessed_ShouldCancelWithoutRestocking()
	{
		// Given
		var quoteId = QuoteId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());

		var orderData = new OrderData(customerId, quoteId);
		var order = Order.Place(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.ShipmentNotDelivered);
		var cancelOrderHandler = new CancelOrderHandler(
			_orderNotificationService, _productInventoryHandler, orderWriteRepository, Substitute.For<IMessageBus>());

		// When
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

		// Then
		var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(canceledOrder);
		Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
		await _productInventoryHandler.DidNotReceive()
			.IncreaseQuantityInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task CancelOrder_WhenAlreadyProcessed_ShouldRestock()
	{
		// Given
		var productId = ProductId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());
		var currency = Currency.OfCode(Currency.USDollar.Code);
		var quoteId = QuoteId.Of(Guid.NewGuid());

		var orderData = new OrderData(customerId, quoteId, currency, new List<ProductItemData>() {
			new ProductItemData() {
				ProductId = productId,
				ProductName = "Product XYZ",
				Quantity = 3,
				UnitPrice = Money.Of(10, currency.Code)
			}
		});
		var order = Order.Place(orderData);
		order.Process(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.ShipmentNotDelivered);
		var cancelOrderHandler = new CancelOrderHandler(
			_orderNotificationService, _productInventoryHandler, orderWriteRepository, Substitute.For<IMessageBus>());

		// When
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

		// Then
		var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
		await _productInventoryHandler.Received(1)
			.IncreaseQuantityInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<CancellationToken>());
	}

	private IOrderNotificationService _orderNotificationService = Substitute.For<IOrderNotificationService>();
	private IProductInventoryHandler _productInventoryHandler = Substitute.For<IProductInventoryHandler>();
}
