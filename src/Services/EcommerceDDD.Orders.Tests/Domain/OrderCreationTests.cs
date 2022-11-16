using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Orders.Tests.Domain;

public class OrderCreationTests
{
    [Fact]
    public void Create_FromQuote_ReturnsOrder()
    {
        // Given
        var productId = ProductId.Of(Guid.NewGuid());
        var productPrice = Money.Of(15, Currency.USDollar.Code);
        const string productName = "Product XYZ";

        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var orderData = new OrderData(_quoteId, _customerId, quoteItems, currency);

        // When
        var order = Order.Create(orderData);

        // Then
        Assert.NotNull(order);
        order.QuoteId.Value.Should().Be(_quoteId.Value);
        order.OrderLines.Count().Should().Be(quoteItems.Count);
        order.CustomerId.Value.Should().Be(_customerId.Value);
        order.Status.Should().Be(OrderStatus.Placed);
    }

    [Fact]
    public void Create_WithEmptyItems_ThrowsException()
    {
        // Given
        var items = new List<ProductItemData>();

        var orderData = new OrderData(_quoteId, _customerId, items, currency);

        // When
        Func<Order> action = () =>
            Order.Create(orderData);

        // Then
        action.Should().Throw<BusinessRuleException>();
    }

    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
    private Currency currency = Currency.OfCode(Currency.USDollar.Code);
}