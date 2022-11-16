using EcommerceDDD.Payments.Domain;

namespace EcommerceDDD.Payments.Tests.Domain;

public class PaymentCreationTests
{
    [Fact]
    public void Create_FromOrder_ReturnsPayment()
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
}