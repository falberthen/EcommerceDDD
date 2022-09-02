using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Tests.Domain;

public class PaymentEventsTests
{
    [Fact]
    public void RequestedPayment_ReturnsPaymentRequestedEvent()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var totalAmount = Money.Of(100, Currency.USDollar.Code);

        // When
        var payment = Payment.CreateNew(orderId, totalAmount);

        // Then
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentRequested;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PaymentRequested>();
    }

    [Fact]
    public void ProcessPayment_ReturnsPaymentProcessedEvent()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var totalAmount = Money.Of(100, Currency.USDollar.Code);
        var payment = Payment.CreateNew(orderId, totalAmount);

        // When
        payment.RecordProcessement();

        // Then
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentProcessed;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PaymentProcessed>();
    }
}