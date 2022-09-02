using Moq;
using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;
using EcommerceDDD.Orders.Application.Quotes;

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

        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        }.ToList();

        var confirmedQuote = new ConfirmedQuote(quoteId, customerId, quoteItems, currency);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();

        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));

        var placeOrder = new PlaceOrder(orderId, confirmedQuote);
        var placeOrderHandler = new PlaceOrderHandler(orderWriteRepository, _checker.Object, _mediator.Object);

        // When
        await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

        // Then
        var placedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        placedOrder.CustomerId.Should().Be(customerId);
        placedOrder.QuoteId.Should().Be(quoteId);
        placedOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        placedOrder.Status.Should().Be(OrderStatus.Placed);

    }

    private Mock<IOrderProductsChecker> _checker = new();
    private Mock<IMediator> _mediator = new();
}