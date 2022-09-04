using EcommerceDDD.Core.Testing;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Application.RequestingPayment;

namespace EcommerceDDD.Payments.Tests.Application;

public class RequestPaymentHandlerTests
{
    [Fact]
    public async Task RequestPayment_WithCommand_ShouldCreatePayment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var totalAmount = Money.Of(100, currency.Code);

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();

        var requestPayment = new RequestPayment(customerId, orderId, totalAmount, currency);
        var requestPaymentHandler = new RequestPaymentHandler(paymentWriteRepository);

        // When
        await requestPaymentHandler.Handle(requestPayment, CancellationToken.None);

        // Then
        var payment = paymentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(payment);
        payment.OrderId.Should().Be(orderId);
        payment.ProcessedAt.Should().Be(null);
        payment.TotalAmount.Value.Should().Be(totalAmount.Value);
        payment.Status.Should().Be(PaymentStatus.Pending);
    }
}