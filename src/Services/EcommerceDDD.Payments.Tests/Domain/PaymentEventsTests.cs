using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Tests.Domain;

public class PaymentEventsTests
{
    [Fact]
    public void RequestedPayment_WithPaymentData_ReturnsPaymentRequestedEvent()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var orderId = OrderId.Of(Guid.NewGuid());
        var totalAmount = Money.Of(100, Currency.USDollar.Code);
        var paymentData = new PaymentData(
            customerId,
            orderId,
            totalAmount);

        // When
        var payment = Payment.Create(paymentData);

        // Then
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentRequested;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PaymentRequested>();
    }

    [Fact]
    public void ProcessPayment_WithPaymentData_ReturnsPaymentProcessedEvent()
    {
        // Given
        var customerId = CustomerId.Of(Guid.NewGuid());
        var orderId = OrderId.Of(Guid.NewGuid());
        var totalAmount = Money.Of(100, Currency.USDollar.Code);
        var paymentData = new PaymentData(
            customerId,
            orderId,
            totalAmount);
        var payment = Payment.Create(paymentData);

        // When
        payment.RecordProcessement();

        // Then
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentProcessed;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PaymentProcessed>();
    }
}