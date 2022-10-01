using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.SignalR;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;
using EcommerceDDD.Orders.Application.Orders.CompletingOrder;
using EcommerceDDD.Orders.Application.Orders.RecordingPayment;
using EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

namespace EcommerceDDD.Orders.Tests.Application;

public class CompleteOrderHandlerTests
{
    [Fact]
    public async Task CompleteOrder_WithCommand_ShouldCompleteOrder()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var orderId = OrderId.Of(Guid.NewGuid());
        var productId = ProductId.Of(Guid.NewGuid());
        var shipmentId = ShipmentId.Of(Guid.NewGuid());
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

        var orderData = new OrderData(orderId, quoteId, customerId, quoteItems, currency);
        var orderWriteRepository = new DummyEventStoreRepository<Order>();

        _productItemsChecker.Setup(p => p.EnsureProductItemsExist(orderData.Items, orderData.Currency))
            .Returns(Task.FromResult(true));

        var responseShipment = new IntegrationHttpResponse() { Success = true };
        _integrationHttpService.Setup(p =>
            p.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.FromResult(responseShipment));

        // Placement
        var placeOrder = PlaceOrder.Create(orderData);
        var placeOrderHandler = new PlaceOrderHandler(orderWriteRepository, _productItemsChecker.Object);
        await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

            // Payment
        var totalPaid = Money.Of(100, currency.Code);
        var paymentId = PaymentId.Of(Guid.NewGuid());
        var recordPaymentToOrder = RecordPayment.Create(paymentId, orderId, totalPaid);
        var recordPaymentToOrderHandler = new RecordPaymentHandler(orderWriteRepository);
        await recordPaymentToOrderHandler.Handle(recordPaymentToOrder, CancellationToken.None);

            // Shipment
        var requestShipment = RequestShipment.Create(orderId);
        var requestShipmentHandler = new RequestShipmentHandler(_integrationHttpService.Object, _orderStatusBroadcaster.Object,
            orderWriteRepository);
        await requestShipmentHandler.Handle(requestShipment, CancellationToken.None);

            // Completion
        var completeOrder = CompleteOrder.Create(orderId, shipmentId);
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
    private Mock<IIntegrationHttpService> _integrationHttpService = new();
    private Mock<IProductItemsChecker> _productItemsChecker = new();
}