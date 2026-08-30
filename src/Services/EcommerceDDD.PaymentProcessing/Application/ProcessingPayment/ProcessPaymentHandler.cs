namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class ProcessPaymentHandler(
	IProductInventoryHandler productInventoryHandler,
	ICustomerCreditChecker creditChecker,
	IEventStoreRepository<Payment> paymentWriteRepository
)
{
	private readonly ICustomerCreditChecker _creditChecker = creditChecker;
	private readonly IProductInventoryHandler _productInventoryHandler = productInventoryHandler;
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(ProcessPayment command, CancellationToken cancellationToken)
	{
		var payment = await _paymentWriteRepository
			.FetchForWritingAsync(command.PaymentId.Value, cancellationToken: cancellationToken);

		if (payment is null)
			return Result.Fail($"Payment {command.PaymentId.Value} was not found.");

		INotification integrationEvent;
		var result = Result.Ok();

		try
		{
			if (!await _creditChecker
				.CheckIfCreditIsEnoughAsync(payment.CustomerId, payment.TotalAmount, cancellationToken))
			{
				payment.Cancel(PaymentCancellationReason.CustomerReachedCreditLimit);
				integrationEvent = new CustomerReachedCreditLimit(payment.OrderId.Value);
			}
			else if (!await _productInventoryHandler
				.CheckProductsInStockAsync(payment.ProductItems, cancellationToken))
			{
				payment.Cancel(PaymentCancellationReason.ProductOutOfStock);
				integrationEvent = new ProductWasOutOfStock(payment.OrderId.Value);
			}
			else
			{
				await _productInventoryHandler
					.DecreaseQuantityInStockAsync(payment.ProductItems, cancellationToken);

				payment.Complete();
				integrationEvent = new PaymentFinalized(
					payment.Id.Value,
					payment.OrderId.Value,
					payment.TotalAmount.Amount,
					payment.TotalAmount.Currency.Code,
					payment.CompletedAt!.Value);
			}
		}
		catch (Exception)
		{
			payment.Cancel(PaymentCancellationReason.ProcessmentError);
			integrationEvent = new PaymentFailed(
				payment.Id.Value,
				payment.OrderId.Value,
				payment.TotalAmount.Amount,
				payment.TotalAmount.Currency.Code);

			result = Result.Fail($"An unexpected error occurred processing payment {command.PaymentId}.");
		}

		await _paymentWriteRepository
			.AppendEventsAndCommitAsync(payment, cancellationToken, integrationEvent);

		return result;
	}
}
