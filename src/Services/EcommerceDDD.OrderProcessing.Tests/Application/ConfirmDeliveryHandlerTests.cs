using EcommerceDDD.OrderProcessing.Application.Orders.ConfirmingDelivery;

namespace EcommerceDDD.OrderProcessing.Tests.Application;

public class ConfirmDeliveryHandlerTests
{
	[Fact]
	public async Task ConfirmDelivery_WithCommand_ShouldDeliverOrder()
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

		var orderNotificationService = Substitute.For<IOrderNotificationService>();

		var confirmDelivery = ConfirmDelivery.Create(order.Id);
		var confirmDeliveryHandler = new ConfirmDeliveryHandler(orderNotificationService, orderWriteRepository);

		// When
		await confirmDeliveryHandler.HandleAsync(confirmDelivery, CancellationToken.None);

		// Then
		var completedOrder = orderWriteRepository.AggregateStream.First().Aggregate;

		Assert.NotNull(completedOrder);
		Assert.Equal(completedOrder.CustomerId, customerId);
		Assert.Equal(completedOrder.QuoteId, quoteId);
		Assert.Equal(completedOrder.PaymentId, paymentId);
		Assert.Equal(completedOrder.ShipmentId, shipmentId);
		Assert.Equal(completedOrder.OrderLines.Count, quoteItems.Count);
		Assert.Equal(OrderStatus.Delivered, completedOrder.Status);
	}
}