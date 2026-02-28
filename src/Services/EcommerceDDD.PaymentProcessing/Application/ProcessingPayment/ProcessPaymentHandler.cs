namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class ProcessPaymentHandler(
	IProductInventoryHandler productInventoryHandler,
	ICustomerCreditChecker creditChecker,
	IEventStoreRepository<Payment> paymentWriteRepository
) : ICommandHandler<ProcessPayment>
{
	private readonly ICustomerCreditChecker _creditChecker = creditChecker;
	private readonly IProductInventoryHandler _productInventoryHandler = productInventoryHandler;
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(ProcessPayment command, CancellationToken cancellationToken)
	{
		var payment = await _paymentWriteRepository
			.FetchStreamAsync(command.PaymentId.Value, cancellationToken: cancellationToken);

		if (payment is null)
			return Result.Fail($"Payment {command.PaymentId} was not found.");

		Activity.Current?.SetTag("order.id", payment.OrderId.Value.ToString());
		try
		{
			if (!await _creditChecker
				.CheckIfCreditIsEnoughAsync(payment.CustomerId, payment.TotalAmount, cancellationToken))
			{
				await CancelPaymentAsync(payment, PaymentCancellationReason.CustomerReachedCreditLimit, cancellationToken);
				return Result.Ok();
			}

			bool allProductsAreAvailable = await _productInventoryHandler
				.CheckProductsInStockAsync(payment.ProductItems, cancellationToken);
			if (!allProductsAreAvailable)
			{
				await CancelPaymentAsync(payment, PaymentCancellationReason.ProductOutOfStock, cancellationToken);
				return Result.Ok();
			}

			await _productInventoryHandler
				.DecreaseQuantityInStockAsync(payment.ProductItems, cancellationToken);

			payment.Complete();

			_paymentWriteRepository.AppendToOutbox(
			   new PaymentFinalized(
				   payment.Id.Value,
				   payment.OrderId.Value,
				   payment.TotalAmount.Amount,
				   payment.TotalAmount.Currency.Code,
				   payment.CompletedAt!.Value));

			await _paymentWriteRepository
				.AppendEventsAsync(payment, cancellationToken);

			return Result.Ok();
		}
		catch (Exception)
		{
			payment.Cancel(PaymentCancellationReason.ProcessmentError);

			_paymentWriteRepository.AppendToOutbox(
				new PaymentFailed(
					payment.Id.Value,
					payment.OrderId.Value,
					payment.TotalAmount.Amount,
					payment.TotalAmount.Currency.Code));

			await _paymentWriteRepository
				.AppendEventsAsync(payment, cancellationToken);

			return Result.Fail($"An unexpected error occurred processing payment {command.PaymentId}.");
		}
	}

	private async Task CancelPaymentAsync(Payment payment, PaymentCancellationReason reason,
		CancellationToken cancellationToken)
	{
		payment.Cancel(reason);

		if (reason == PaymentCancellationReason.CustomerReachedCreditLimit)
		{
			_paymentWriteRepository.AppendToOutbox(
				new CustomerReachedCreditLimit(payment.OrderId.Value));
		}
		if (reason == PaymentCancellationReason.ProductOutOfStock)
		{
			_paymentWriteRepository.AppendToOutbox(
				new ProductWasOutOfStock(payment.OrderId.Value));
		}

		await _paymentWriteRepository
			.AppendEventsAsync(payment, cancellationToken);
	}
}
