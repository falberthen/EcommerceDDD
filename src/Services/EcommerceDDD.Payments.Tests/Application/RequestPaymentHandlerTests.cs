using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Integration;
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
        var availableCreditLimit = 1000;

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();

        var response = new IntegrationHttpResponse<CreditLimitModel>()
        {
            Success = true,
            Data = new CreditLimitModel(customerId.Value, availableCreditLimit)
        };

        _customerCreditChecker.Setup(c => c.EnsureEnoughCredit(It.IsAny<CustomerId>(), It.IsAny<Money>()))
            .Returns(Task.FromResult(true));

        var requestPayment = RequestPayment.Create(customerId, orderId, totalAmount, currency);
        var requestPaymentHandler = new RequestPaymentHandler(_eventProducer.Object, _customerCreditChecker.Object,
            paymentWriteRepository);

        // When
        await requestPaymentHandler.Handle(requestPayment, CancellationToken.None);

        // Then
        var payment = paymentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(payment);
        payment.OrderId.Should().Be(orderId);
        payment.ProcessedAt.Should().NotBe(null);
        payment.TotalAmount.Amount.Should().Be(totalAmount.Amount);
        payment.Status.Should().Be(PaymentStatus.Processed);
    }

    private Mock<IEventProducer> _eventProducer = new();
    private Mock<ICustomerCreditChecker> _customerCreditChecker = new();
}