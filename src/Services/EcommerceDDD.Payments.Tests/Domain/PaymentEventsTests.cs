using PaymentCompleted = EcommerceDDD.Payments.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.Payments.Tests.Domain;

public class PaymentEventsTests
{
    [Fact]
    public void RequestedPayment_WithPaymentData_ReturnsPaymentCreatedEvent()
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
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentCreated;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PaymentCreated>();
    }

    [Fact]
    public void CompletePayment_WithPaymentData_ReturnsPaymentCompletedEvent()
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
        payment.Complete();

        // Then
        var @event = payment.GetUncommittedEvents().LastOrDefault() as PaymentCompleted;
        Assert.NotNull(@event);
        @event.Should().BeOfType<PaymentCompleted>();
    }
}