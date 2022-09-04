using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using Microsoft.Extensions.Options;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.Orders.Application.Orders.RecordingPayment;

namespace EcommerceDDD.Orders.Tests.Application;

public class RecordPaymentToOrderHandlerTests
{
    [Fact]
    public async Task RecordPaymentToOrder_WithCommand_ShouldRecordOrderPaid()
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

        var serviceProvider = DummyServiceProvider.Setup();
        serviceProvider
            .Setup(x => x.GetService(typeof(IEventStoreRepository<Order>)))
            .Returns(orderWriteRepository);

        _ordersService.Setup(p => p.UpdateOrderStatus(It.IsAny<string>(), It.IsAny<UpdateOrderStatusRequest>()))
            .Returns(Task.CompletedTask);

        serviceProvider
            .Setup(x => x.GetService(typeof(IOrdersService)))
            .Returns(_ordersService.Object);

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var recordPaymentToOrder = new RecordPaymentToOrder(paymentId, orderId, totalPaid);
        var recordPaymentToOrderHandler = new RecordPaymentToOrderHandler(_mediator.Object, serviceProvider.Object, options.Object);

        // When
        await recordPaymentToOrderHandler.Handle(recordPaymentToOrder, CancellationToken.None);

        // Then
        var paidOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        paidOrder.Id.Should().Be(orderId);
        paidOrder.PaymentId.Should().Be(paymentId);
        paidOrder.Status.Should().Be(OrderStatus.Paid);
    }
    
    private Mock<IOrdersService> _ordersService = new();
    private Mock<IOrderProductsChecker> _checker = new();
    private Mock<IMediator> _mediator = new();
}