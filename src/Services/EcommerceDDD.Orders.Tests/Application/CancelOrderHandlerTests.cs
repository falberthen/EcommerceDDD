using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.Orders.Application.Orders.CancelingOrder;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.IntegrationServices.Orders;

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
        await placeOrderHandler.Handle(placeOrder, CancellationToken.None);

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var serviceProvider = DummyServiceProvider.Setup();
        serviceProvider
            .Setup(x => x.GetService(typeof(IEventStoreRepository<Order>)))
            .Returns(orderWriteRepository);

        serviceProvider
            .Setup(x => x.GetService(typeof(IOrdersService)))
            .Returns(_ordersService.Object);

        var cancelOrder = new CancelOrder(orderId, OrderCancellationReason.CanceledByUser);
        var cancelOrderHandler = new CancelOrderHandler(serviceProvider.Object, options.Object);

        // When
        await cancelOrderHandler.Handle(cancelOrder, CancellationToken.None);

        // Then
        var canceledOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        canceledOrder.CustomerId.Should().Be(customerId);
        canceledOrder.QuoteId.Should().Be(quoteId);
        canceledOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        canceledOrder.Status.Should().Be(OrderStatus.Canceled);
    }

    private Mock<IOrdersService> _ordersService = new();
    private Mock<IOrderProductsChecker> _checker = new();
    private Mock<IMediator> _mediator = new();
}