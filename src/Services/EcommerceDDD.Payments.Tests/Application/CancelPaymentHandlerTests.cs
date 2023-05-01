namespace EcommerceDDD.Payments.Tests.Application;

public class CancelPaymentHandlerTests
{
    [Fact]
    public async Task CancelPayment_WithCommand_ShouldCancelPayment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var totalAmount = Money.Of(100, currency.Code);
        var payment = Payment.Create(new PaymentData(customerId, orderId, totalAmount));

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();
        await paymentWriteRepository.AppendEventsAsync(payment);

        var cancelPayment = CancelPayment.Create(payment.Id, PaymentCancellationReason.OrderCanceled);
        var cancelPaymentHandler = new CancelPaymentHandler(paymentWriteRepository);

        // When
        await cancelPaymentHandler.Handle(cancelPayment, CancellationToken.None);

        // Then
        Assert.NotNull(payment);
        payment.OrderId.Should().Be(orderId);
        payment.CreatedAt.Should().NotBe(null);
        payment.CompletedAt.Should().Be(null);
        payment.CanceledAt.Should().NotBe(null);        
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.Status.Should().Be(PaymentStatus.Canceled);
    }
}