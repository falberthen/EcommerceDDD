using MediatR;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Application.RequestingPayment;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Payments.Application.CompletingPayment;

namespace EcommerceDDD.Payments.Tests.Application;

public class CompletePaymentHandlerTests
{
    [Fact]
    public async Task ProcessPayment_WithDomainEvent_ShouldProcessPayment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var totalAmount = Money.Of(100, currency.Code);

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();

        var requestPayment = new RequestPayment(customerId, orderId, totalAmount, currency);
        var requestPaymentHandler = new RequestPaymentHandler(_mediator.Object, paymentWriteRepository);
        await requestPaymentHandler.Handle(requestPayment, CancellationToken.None);

        var payment = paymentWriteRepository.AggregateStream.First().Aggregate;
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentRequested;
        Assert.NotNull(@event);

        var completePaymentHandler = new ProcessPaymentHandler(_mediator.Object, paymentWriteRepository);

        // When
        await completePaymentHandler.Handle(@event!, CancellationToken.None);

        // Then
        Assert.NotNull(payment);
        payment.OrderId.Should().Be(orderId);
        payment.ProcessedAt.Should().NotBe(null);
        payment.TotalAmount.Value.Should().Be(totalAmount.Value);
        payment.Status.Should().Be(PaymentStatus.Processed);
    }

    private Mock<IMediator> _mediator = new();
}