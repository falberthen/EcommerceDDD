using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Orders.Application.Orders.PlacingOrder;
using EcommerceDDD.Orders.Application.Quotes;
using EcommerceDDD.Orders.Application.Orders.CancelingOrder;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.Orders.Application.Orders.CompletingOrder;
using EcommerceDDD.Orders.Application.Orders.RecordingPayment;
using EcommerceDDD.Orders.Application.Shipments.RequestingShipment;
using EcommerceDDD.IntegrationServices.Shipments;

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
        var productName = "Product XYZ";
        var productPrice = Money.Of(10, Currency.USDollar.Code);
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);

        var quoteItems = new List<ConfirmedQuoteItem>() {
            new ConfirmedQuoteItem() {
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

            // Placement
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
        serviceProvider
            .Setup(x => x.GetService(typeof(IShipmentsService)))
            .Returns(_shipmentsService.Object);
        
            // Payment
        var totalPaid = Money.Of(100, currency.Code);
        var paymentId = PaymentId.Of(Guid.NewGuid());
        var recordPaymentToOrder = new RecordPaymentToOrder(paymentId, orderId, totalPaid);
        var recordPaymentToOrderHandler = new RecordPaymentToOrderHandler(_mediator.Object, serviceProvider.Object);
        await recordPaymentToOrderHandler.Handle(recordPaymentToOrder, CancellationToken.None);

            // Shipment
        var orderLines = orderWriteRepository.AggregateStream.First()
            .Aggregate.OrderLines;
        var requestShipment = new RequestShipment(orderId, orderLines);
        var requestShipmentHandler = new RequestShipmentHandler(serviceProvider.Object, options.Object);
        await requestShipmentHandler.Handle(requestShipment, CancellationToken.None);

            // Completion
        var completeOrder = new CompleteOrder(orderId);
        var completeOrderHandler = new CompleteOrderHandler(serviceProvider.Object, options.Object);

        // When
        await completeOrderHandler.Handle(completeOrder, CancellationToken.None);

        // Then
        var completedOrder = orderWriteRepository.AggregateStream.First().Aggregate;
        completedOrder.CustomerId.Should().Be(customerId);
        completedOrder.QuoteId.Should().Be(quoteId);
        completedOrder.OrderLines.Count.Should().Be(quoteItems.Count);
        completedOrder.Status.Should().Be(OrderStatus.Completed);
    }

    private Mock<IOrdersService> _ordersService = new();
    private Mock<IShipmentsService> _shipmentsService = new();    
    private Mock<IOrderProductsChecker> _checker = new();
    private Mock<IMediator> _mediator = new();
}