using System.Collections.Generic;

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
		var productItems = new List<ProductItem>() {
			new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
		};
		var payment = Payment.Create(new PaymentData(customerId, orderId, totalAmount, productItems));

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();
        await paymentWriteRepository.AppendEventsAsync(payment);

        _customerCreditChecker.
			CheckIfCreditIsEnoughAsync(Arg.Any<CustomerId>(), Arg.Any<Money>(), CancellationToken.None)
           .Returns(Task.FromResult(true));
		_productInventoryHandler
			.CheckProductsInStockAsync(Arg.Any<IReadOnlyList<ProductItem>>(), CancellationToken.None)
			.Returns(Task.FromResult(true));

		// When
		var processPayment = ProcessPayment.Create(payment.Id);
		var processPaymentHandler = new ProcessPaymentHandler(
			_productInventoryHandler, _customerCreditChecker, paymentWriteRepository);
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
		var productItems = new List<ProductItem>() {
			new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
			new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
		};
		var payment = Payment.Create(new PaymentData(customerId, orderId, totalAmount, productItems));

        var paymentWriteRepository = new DummyEventStoreRepository<Payment>();
        await paymentWriteRepository.AppendEventsAsync(payment);

        _customerCreditChecker
            .CheckIfCreditIsEnoughAsync(Arg.Any<CustomerId>(), Arg.Any<Money>(), CancellationToken.None)
           .Returns(Task.FromResult(false));
		_productInventoryHandler
			.CheckProductsInStockAsync(Arg.Any<IReadOnlyList<ProductItem>>(), CancellationToken.None)
			.Returns(Task.FromResult(true));

		var processPayment = ProcessPayment.Create(payment.Id);
        var processPaymentHandler = new ProcessPaymentHandler(
			_productInventoryHandler, _customerCreditChecker, paymentWriteRepository);

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
	private IProductInventoryHandler _productInventoryHandler = Substitute.For<IProductInventoryHandler>();
}