using Moq;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.Orders.Domain.Events;

namespace EcommerceDDD.Orders.Tests.Domain;

public class OrderEventsTests
{
    [Fact]
    public async Task PlacingOrder_ReturnsOrderPlacedEvent()
    {
        // Given        
        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
                Id = Guid.NewGuid(),
                ProductId = _productId,
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
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderPlaced;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderPlaced>();
    }

    [Fact]
    public async Task CancelingOrder_ReturnsOrderCanceledEvent()
    {
        // Given        
        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
                Id = Guid.NewGuid(),
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var confirmedQuote = new ConfirmedQuote(_quoteId, _customerId, quoteItems, currency);
        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));
        var order = await Order.CreateNew(_orderId, confirmedQuote, _checker.Object);

        // When
        order.Cancel(OrderCancellationReason.CanceledByUser);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderCanceled;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderCanceled>();
    }

    [Fact]
    public async Task PayingOrder_ReturnsOrderPaidEvent()
    {
        // Given
        var totalPaid = Money.Of(100, Currency.USDollar.Code);

        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
                Id = Guid.NewGuid(),
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var confirmedQuote = new ConfirmedQuote(_quoteId, _customerId, quoteItems, currency);
        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));
        var order = await Order.CreateNew(_orderId, confirmedQuote, _checker.Object);

        // When
        order.RecordPayment(_paymentId, totalPaid);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderPaid;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderPaid>();
    }

    [Fact]
    public async Task CompletingOrder_ReturnsOrderCompletedEvent()
    {
        // Given
        var totalPaid = Money.Of(100, Currency.USDollar.Code);

        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
                Id = Guid.NewGuid(),
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var confirmedQuote = new ConfirmedQuote(_quoteId, _customerId, quoteItems, currency);
        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));

        var order = await Order.CreateNew(_orderId, confirmedQuote, _checker.Object);
        order.RecordPayment(_paymentId, totalPaid);

        // When
        order.Complete();

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderCompleted;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderCompleted>();
    }

    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private OrderId _orderId = OrderId.Of(Guid.NewGuid());
    private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private PaymentId _paymentId = PaymentId.Of(Guid.NewGuid());
    const string productName = "Product XYZ";
    private Currency currency = Currency.OfCode(Currency.USDollar.Code);
    private Money productPrice = Money.Of(15, Currency.USDollar.Code);
    private Mock<IOrderProductsChecker> _checker = new();
}