namespace EcommerceDDD.PaymentProcessing.Tests.Application;

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
        var requestPaymentHandler = new RequestPaymentHandler(_commandBus, paymentWriteRepository);

        // When
        await requestPaymentHandler.HandleAsync(requestPayment, CancellationToken.None);

        // Then
        var payment = paymentWriteRepository.AggregateStream.First().Aggregate;        
		Assert.NotNull(payment);
		Assert.Equal(payment.OrderId, orderId);
		Assert.NotNull(payment.CreatedAt);
		Assert.Null(payment.CompletedAt);
		Assert.Equal(payment.TotalAmount.Amount, totalAmount.Amount);
		Assert.Equal(PaymentStatus.Pending, payment.Status);
	}

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
}