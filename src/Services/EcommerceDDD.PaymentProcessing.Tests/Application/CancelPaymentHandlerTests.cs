namespace EcommerceDDD.PaymentProcessing.Tests.Application;

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
		var productItems = new List<ProductItem>() {
			new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
		};
		var payment = Payment.Create(new PaymentData(customerId, orderId, totalAmount, productItems));

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();
        await paymentWriteRepository.AppendEventsAsync(payment);

        var cancelPayment = CancelPayment.Create(payment.Id, PaymentCancellationReason.OrderCanceled);
        var cancelPaymentHandler = new CancelPaymentHandler(paymentWriteRepository);

        // When
        await cancelPaymentHandler.HandleAsync(cancelPayment, CancellationToken.None);

        // Then        
		Assert.NotNull(payment);
		Assert.Equal(payment.OrderId, orderId);
		Assert.NotNull(payment.CreatedAt);
		Assert.Null(payment.CompletedAt);
		Assert.NotNull(payment.CanceledAt);
		Assert.Equal(payment.TotalAmount.Amount, totalAmount.Amount);
		Assert.Equal(PaymentStatus.Canceled, payment.Status);
	}
}