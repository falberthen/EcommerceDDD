namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class CancelOrderHandlerTests : IDisposable
{
	[Fact]
	public async Task CancelOrder_WhenNotYetProcessed_ShouldCancelWithoutRestocking()
	{
		// Given
		var quoteId = QuoteId.Of(Guid.NewGuid());
		var customerId = CustomerId.Of(Guid.NewGuid());

		var orderData = new OrderData(customerId, quoteId);
		var order = Order.Place(OrderId.Of(Guid.NewGuid()), orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.ShipmentNotDelivered);
		var cancelOrderHandler = new CancelOrderHandler(
			_orderNotificationService, _productInventoryHandler, orderWriteRepository, _orderMetrics.Metrics);

		// When
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

		// Then
		var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(canceledOrder);
		Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
		Assert.Single(orderWriteRepository.PublishedMessages.OfType<OrderCanceled>());
		await _productInventoryHandler.DidNotReceive()
			.IncreaseQuantityInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), Arg.Any<OrderId>(), Arg.Any<CancellationToken>());
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
		var order = Order.Place(OrderId.Of(Guid.NewGuid()), orderData);
		order.Process(orderData);

		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.ShipmentNotDelivered);
		var cancelOrderHandler = new CancelOrderHandler(
			_orderNotificationService, _productInventoryHandler, orderWriteRepository, _orderMetrics.Metrics);

		// When
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

		// Then
		var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
		Assert.Equal(OrderStatus.Canceled, canceledOrder.Status);
		await _productInventoryHandler.Received(1)
			.IncreaseQuantityInStockAsync(Arg.Any<IReadOnlyList<ProductItemData>>(), order.Id, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task CancelOrder_WhenRetried_ShouldCountCanceledOrderOnceWithReason_AndNoOrderId()
	{
		// Given
		var order = Order.Place(OrderId.Of(Guid.NewGuid()), new OrderData(CustomerId.Of(Guid.NewGuid()), QuoteId.Of(Guid.NewGuid())));
		var orderWriteRepository = new DummyEventStoreRepository<Order>();
		await orderWriteRepository.AppendEventsAndCommitAsync(order);

		var cancelOrder = CancelOrder.Create(order.Id, OrderCancellationReason.ProductWasOutOfStock);
		var cancelOrderHandler = new CancelOrderHandler(
			_orderNotificationService, _productInventoryHandler, orderWriteRepository, _orderMetrics.Metrics);

		// When: the same message is handled twice, as a Wolverine retry would
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);
		await cancelOrderHandler.HandleAsync(cancelOrder, CancellationToken.None);

		// Then
		var measurement = Assert.Single(_orderMetrics.Measurements);
		Assert.Equal(1, measurement.Value);
		Assert.Equal("canceled", measurement.Tags["outcome"]);
		Assert.Equal("out_of_stock", measurement.Tags["reason"]);
		Assert.Equal(2, measurement.Tags.Count);
	}

	public void Dispose() => _orderMetrics.Dispose();

	private IOrderNotificationService _orderNotificationService = Substitute.For<IOrderNotificationService>();
	private IProductInventoryHandler _productInventoryHandler = Substitute.For<IProductInventoryHandler>();
	private OrderMetricsRecorder _orderMetrics = new();
}
