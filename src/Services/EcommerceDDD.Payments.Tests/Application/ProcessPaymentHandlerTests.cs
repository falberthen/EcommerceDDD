namespace EcommerceDDD.Payments.Tests.Application;

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
        var processPaymentHandler = new ProcessPaymentHandler(_customerCreditChecker, 
            paymentWriteRepository, _outboxMessageService);
        await processPaymentHandler.Handle(processPayment, CancellationToken.None);

        // Then
        var completedPayment = paymentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(completedPayment);
        payment.OrderId.Should().Be(orderId);
        payment.CreatedAt.Should().NotBe(null);
        payment.CompletedAt.Should().NotBe(null);
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.Status.Should().Be(PaymentStatus.Completed);
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
        var processPaymentHandler = new ProcessPaymentHandler(_customerCreditChecker, 
            paymentWriteRepository, _outboxMessageService);

        // When
        await processPaymentHandler.Handle(processPayment, CancellationToken.None);

        // Then
        var canceledPayment = paymentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(canceledPayment);
        payment.OrderId.Should().Be(orderId);
        payment.CreatedAt.Should().NotBe(null);
        payment.CompletedAt.Should().Be(null);
        payment.CanceledAt.Should().NotBe(null);
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.Status.Should().Be(PaymentStatus.Canceled);
    }

    private IOutboxMessageService _outboxMessageService = Substitute.For<IOutboxMessageService>();
    private ICustomerCreditChecker _customerCreditChecker = Substitute.For<ICustomerCreditChecker>();
}