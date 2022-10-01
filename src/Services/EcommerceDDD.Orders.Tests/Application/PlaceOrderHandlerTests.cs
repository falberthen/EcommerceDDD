using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;

namespace EcommerceDDD.Orders.Tests.Application;

public class PlaceOrderHandlerTests
{
    [Fact]
    public async Task PlaceOrder_WithCommand_ShouldPlaceOrder()
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
        };

        var orderData = new OrderData(orderId, quoteId, customerId, quoteItems, currency);
        
        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        _productItemsChecker.Setup(p => p.EnsureProductItemsExist(orderData.Items, orderData.Currency))
            .Returns(Task.FromResult(true));

        var placeOrder = PlaceOrder.Create(orderData);
        var placeOrderHandler = new PlaceOrderHandler(orderWriteRepository, _productItemsChecker.Object);

        // When
        await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

        // Then
        var placedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        placedOrder.CustomerId.Should().Be(customerId);
        placedOrder.QuoteId.Should().Be(quoteId);
        placedOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        placedOrder.Status.Should().Be(OrderStatus.Placed);

    }

    private Mock<IProductItemsChecker> _productItemsChecker = new();
}