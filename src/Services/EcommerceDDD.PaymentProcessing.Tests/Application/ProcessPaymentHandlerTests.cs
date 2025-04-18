namespace EcommerceDDD.PaymentProcessing.Tests.Application;

public class ProcessPaymentHandlerTests
{
    [Fact]
    public async Task ProcessPayment_WithCommand_ShouldCompletePayment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var totalAmount = Money.Of(100, currency.Code);
        var payment = Payment.Create(new PaymentData(customerId, orderId, totalAmount));

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();
        await paymentWriteRepository.AppendEventsAsync(payment);

        _customerCreditChecker.IsCreditEnough(Arg.Any<CustomerId>(), Arg.Any<Money>())
           .Returns(Task.FromResult(true));

        // When
        var processPayment = ProcessPayment.Create(payment.Id);
        var processPaymentHandler = new ProcessPaymentHandler(_customerCreditChecker, paymentWriteRepository);
        await processPaymentHandler.HandleAsync(processPayment, CancellationToken.None);

        // Then
        var completedPayment = paymentWriteRepository.AggregateStream.First().Aggregate;        
		Assert.NotNull(payment);
		Assert.Equal(payment.OrderId, orderId);
		Assert.NotNull(payment.CreatedAt);
		Assert.NotNull(payment.CompletedAt);
		Assert.Equal(payment.TotalAmount.Amount, totalAmount.Amount);
		Assert.Equal(PaymentStatus.Completed, payment.Status);
	}

    [Fact]
    public async Task ProcessPayment_WithoutEnoughCredit_CancelPayment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var customerId = CustomerId.Of(Guid.NewGuid());
        var currency = Currency.OfCode(Currency.USDollar.Code);
        var totalAmount = Money.Of(100, currency.Code);
        var payment = Payment.Create(new PaymentData(customerId, orderId, totalAmount));

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();
        await paymentWriteRepository.AppendEventsAsync(payment);

        _customerCreditChecker
            .IsCreditEnough(Arg.Any<CustomerId>(), Arg.Any<Money>())
           .Returns(Task.FromResult(false));

        var processPayment = ProcessPayment.Create(payment.Id);
        var processPaymentHandler = new ProcessPaymentHandler(_customerCreditChecker, paymentWriteRepository);

        // When
        await processPaymentHandler.HandleAsync(processPayment, CancellationToken.None);

        // Then
        var canceledPayment = paymentWriteRepository.AggregateStream.First().Aggregate;
		Assert.NotNull(payment);
		Assert.Equal(payment.OrderId, orderId);
		Assert.NotNull(payment.CreatedAt);
		Assert.Null(payment.CompletedAt);
		Assert.NotNull(payment.CanceledAt);
		Assert.Equal(payment.TotalAmount.Amount, totalAmount.Amount);
		Assert.Equal(PaymentStatus.Canceled, payment.Status);
	}

    private ICustomerCreditChecker _customerCreditChecker = Substitute.For<ICustomerCreditChecker>();
}