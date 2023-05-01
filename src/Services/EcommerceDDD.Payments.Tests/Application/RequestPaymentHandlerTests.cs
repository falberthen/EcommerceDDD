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

        var requestPayment = RequestPayment.Create(customerId, orderId, totalAmount, currency);
        var requestPaymentHandler = new RequestPaymentHandler(_commandBus.Object, paymentWriteRepository);

        // When
        await requestPaymentHandler.Handle(requestPayment, CancellationToken.None);

        // Then
        var payment = paymentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(payment);
        payment.OrderId.Should().Be(orderId);
        payment.CreatedAt.Should().NotBe(null);
        payment.CompletedAt.Should().Be(null);
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    private Mock<ICommandBus> _commandBus = new();
}