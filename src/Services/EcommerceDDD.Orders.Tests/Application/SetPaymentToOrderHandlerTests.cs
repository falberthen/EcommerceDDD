using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.Orders.Application.Orders.SettingPayment;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using Microsoft.Extensions.Options;
using EcommerceDDD.IntegrationServices;

namespace EcommerceDDD.Orders.Tests.Application;

public class SetPaymentToOrderHandlerTests
{
    [Fact]
    public async Task SetPaymentToOrder_WithCommand_ShouldSetOrderPaid()
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

        var totalPaid = Money.Of(100, currency.Code);
        var paymentId = PaymentId.Of(Guid.NewGuid());

        _checker.Setup(p => p.CheckFromQuote(confirmedQuote))
            .Returns(Task.FromResult(true));

        var order = await Order.CreateNew(orderId, confirmedQuote, _checker.Object);

        var orderWriteRepository = new DummyEventStoreRepository<Order>();
        await orderWriteRepository
            .AppendEventsAsync(order, CancellationToken.None);

        SetupScope();
        _serviceProvider
            .Setup(x => x.GetService(typeof(IEventStoreRepository<Order>)))
            .Returns(orderWriteRepository);

        _ordersService.Setup(p => p.UpdateOrderStatus(It.IsAny<string>(), It.IsAny<UpdateOrderStatusRequest>()))
            .Returns(Task.CompletedTask);

        _serviceProvider
            .Setup(x => x.GetService(typeof(IOrdersService)))
            .Returns(_ordersService.Object);

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var setPaymentToOrder = new SetPaymentToOrder(paymentId, orderId, totalPaid);
        var setPaymentToOrderHandler = new SetPaymentToOrderHandler(_mediator.Object, _serviceProvider.Object, options.Object);

        // When
        await setPaymentToOrderHandler.Handle(setPaymentToOrder, CancellationToken.None);

        // Then
        var paidOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        paidOrder.Id.Should().Be(orderId);
        paidOrder.PaymentId.Should().Be(paymentId);
        paidOrder.Status.Should().Be(OrderStatus.Paid);
    }
    
    private void SetupScope()
    {
        var scope = new Mock<IServiceScope>();
        scope.SetupGet(s => s.ServiceProvider)
            .Returns(_serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope())
            .Returns(scope.Object);

        _serviceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);
    }

    private Mock<IOrdersService> _ordersService = new();
    private Mock<IOrderProductsChecker> _checker = new();
    private Mock<IServiceProvider> _serviceProvider = new();
    private Mock<IMediator> _mediator = new();
}