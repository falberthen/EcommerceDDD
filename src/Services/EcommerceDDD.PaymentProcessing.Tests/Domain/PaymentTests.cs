namespace EcommerceDDD.PaymentProcessing.Tests.Domain;

public class PaymentTests
{
    [Fact]
    public void Create_WithPaymentData_ShouldCreatePayment()
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
        Assert.NotNull(payment);
        payment.OrderId.Value.Should().Be(orderId.Value);
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.CompletedAt.Should().Be(null);
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Complete_WithPayment_ShouldCompletePayment()
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
        Assert.NotNull(payment);
        payment.OrderId.Value.Should().Be(orderId.Value);
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.CompletedAt.Should().NotBe(null);
        payment.Status.Should().Be(PaymentStatus.Completed);
    }
}