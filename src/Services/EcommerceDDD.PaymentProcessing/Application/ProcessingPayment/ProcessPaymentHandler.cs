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

	public async Task HandleAsync(ProcessPayment command, CancellationToken cancellationToken)
	{
		var payment = await _paymentWriteRepository
			.FetchStreamAsync(command.PaymentId.Value, cancellationToken: cancellationToken)
			?? throw new RecordNotFoundException($"Payment {command.PaymentId} was not found.");

		try
		{
			// Checking customer credit
			if (!await _creditChecker
				.CheckIfCreditIsEnoughAsync(payment.CustomerId, payment.TotalAmount, cancellationToken))
			{
				await CancelPaymentAsync(payment, PaymentCancellationReason.CustomerReachedCreditLimit, cancellationToken);
				return;
			}

			// Checking if all items are in stock
			bool allProductsAreAvailable = await _productInventoryHandler
				.CheckProductsInStockAsync(payment.ProductItems, cancellationToken);
			if (!allProductsAreAvailable)
			{
				await CancelPaymentAsync(payment, PaymentCancellationReason.ProductOutOfStock, cancellationToken);
				return;
			}

			// Decreasing quantity in stock
			await _productInventoryHandler
				.DecreaseQuantityInStockAsync(payment.ProductItems, cancellationToken);

			// Completing payment
			payment.Complete();

			// Appending to outbox for message broker
			_paymentWriteRepository.AppendToOutbox(
			   new PaymentFinalized(
				   payment.Id.Value,
				   payment.OrderId.Value,
				   payment.TotalAmount.Amount,
				   payment.TotalAmount.Currency.Code,
				   payment.CompletedAt!.Value));

			// Persisting aggregate
			await _paymentWriteRepository
				.AppendEventsAsync(payment, cancellationToken);
		}
		catch (Exception) // Unexpected issue
		{
			payment.Cancel(PaymentCancellationReason.ProcessmentError);

			// Appending integration event to outbox
			_paymentWriteRepository.AppendToOutbox(
				new PaymentFailed(
					payment.Id.Value,
					payment.OrderId.Value,
					payment.TotalAmount.Amount,
					payment.TotalAmount.Currency.Code));

			await _paymentWriteRepository
				.AppendEventsAsync(payment, cancellationToken);
		}
	}

	private async Task CancelPaymentAsync(Payment payment, PaymentCancellationReason reason, 
		CancellationToken cancellationToken)
	{
		payment.Cancel(reason);

		// Appending integration event to outbox
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