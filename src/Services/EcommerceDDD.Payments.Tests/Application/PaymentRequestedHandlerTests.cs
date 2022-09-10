using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Payments.Application.RequestingPayment;
using EcommerceDDD.Payments.Application.ProcessingPayment;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.IntegrationServices.Customers;
using EcommerceDDD.IntegrationServices.Customers.Responses;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Payments.Tests.Application;

public class PaymentRequestedHandlerTests
{
    [Fact]
    public async Task ProcessPayment_WithDomainEventNotification_ShouldProcessPayment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var totalAmount = Money.Of(100, currency.Code);
        var availableCreditLimit = 1000;

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();

        var requestPayment = new RequestPayment(customerId, orderId, totalAmount, currency);
        var requestPaymentHandler = new RequestPaymentHandler(paymentWriteRepository);
        await requestPaymentHandler.Handle(requestPayment, CancellationToken.None);

        var payment = paymentWriteRepository.AggregateStream.First().Aggregate;
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentRequested;
        Assert.NotNull(@event);

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        _customersService.Setup(p => p.RequestAvailableCreditLimit(It.IsAny<string>(), It.IsAny<Guid>()))
            .Returns(Task.FromResult(new AvailableCreditLimitModel(customerId.Value, availableCreditLimit)));

        var serviceProvider = DummyServiceProvider.Setup();
        serviceProvider
            .Setup(x => x.GetService(typeof(IEventStoreRepository<Payment>)))
            .Returns(paymentWriteRepository);

        serviceProvider
            .Setup(x => x.GetService(typeof(IOptions<IntegrationServicesSettings>)))
            .Returns(options.Object);        
        serviceProvider
            .Setup(x => x.GetService(typeof(ICustomersService)))
            .Returns(_customersService.Object);
        serviceProvider
            .Setup(x => x.GetService(typeof(IEventProducer)))
            .Returns(_eventProducer.Object);

        var domainNotification = new DomainEventNotification<PaymentRequested>(@event!);
        var paymentRequestedHandler = new PaymentRequestedHandler(serviceProvider.Object);

        // When
        await paymentRequestedHandler.Handle(domainNotification, CancellationToken.None);

        // Then
        Assert.NotNull(payment);
        payment.OrderId.Should().Be(orderId);
        payment.ProcessedAt.Should().NotBe(null);
        payment.TotalAmount.Value.Should().Be(totalAmount.Value);
        payment.Status.Should().Be(PaymentStatus.Processed);
    }

    private Mock<ICustomersService> _customersService = new();
    private Mock<IEventProducer> _eventProducer = new();
}