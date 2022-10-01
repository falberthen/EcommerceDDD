using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.SignalR;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;
using EcommerceDDD.Orders.Application.Orders.CancelingOrder;

namespace EcommerceDDD.Orders.Tests.Application;

public class CancelOrderHandlerTests
{
    [Fact]
    public async Task CancelOrder_WithCommand_ShouldCancelOrder()
    {
        // Given
        var quoteId = QuoteId.Of(Guid.NewGuid());
        var orderId = OrderId.Of(Guid.NewGuid());
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

        var orderData = new OrderData(orderId, quoteId, customerId, quoteItems, currency);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();

        _productItemsChecker.Setup(p => p.EnsureProductItemsExist(orderData.Items, orderData.Currency))
            .Returns(Task.FromResult(true));

        var placeOrder = PlaceOrder.Create(orderData);
        var placeOrderHandler = new PlaceOrderHandler(orderWriteRepository, _productItemsChecker.Object);
        await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

        var cancelOrder = CancelOrder.Create(orderId, OrderCancellationReason.CanceledByUser);
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
    private Mock<IProductItemsChecker> _productItemsChecker = new();
}