using Moq;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Orders.Tests.Domain;

public class OrderCreationTests
{
    [Fact]
    public async Task Create_FromQuote_ReturnsOrder()
    {
        // Given
        var productId = ProductId.Of(Guid.NewGuid());
        var productPrice = Money.Of(15, Currency.USDollar.Code);
        const string productName = "Product XYZ";

        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var confirmedQuote = new ConfirmedQuote(_quoteId, _customerId, quoteItems, currency);
        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));

        // When
        var order = await Order.CreateNew(_orderId, confirmedQuote, _checker.Object);

        // Then
        Assert.NotNull(order);
        order.QuoteId.Value.Should().Be(_quoteId.Value);
        order.OrderLines.Count().Should().Be(quoteItems.Count);
        order.CustomerId.Value.Should().Be(_customerId.Value);
        order.Status.Should().Be(OrderStatus.Placed);
    }

    [Fact]
    public async Task Create_WithEmptyItems_ThrowsException()
    {
        // Given
        var quoteItems = new List<ConfirmedQuoteItem>();
        var confirmedQuote = new ConfirmedQuote(_quoteId, _customerId, quoteItems, currency);
        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));

        // When
        Func<Task> action = async () =>
            await Order.CreateNew(_orderId, confirmedQuote, _checker.Object);

        // Then
        await action.Should().ThrowAsync<DomainException>();
    }

    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private OrderId _orderId = OrderId.Of(Guid.NewGuid());
    private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
    private Currency currency = Currency.OfCode(Currency.USDollar.Code);
    private Mock<IOrderProductsChecker> _checker = new();
}