namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

public class ProcessPaymentHandler(
	IProductInventoryHandler productInventoryHandler,
	ICustomerStoreCreditChecker storeCreditChecker,
	IEventStoreRepository<Payment> paymentWriteRepository
)
{
	private readonly ICustomerStoreCreditChecker _storeCreditChecker = storeCreditChecker;
	private readonly IProductInventoryHandler _productInventoryHandler = productInventoryHandler;
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(ProcessPayment command, CancellationToken cancellationToken)
	{
		var payment = await _paymentWriteRepository
			.FetchForWritingAsync(command.PaymentId.Value, cancellationToken: cancellationToken);

		if (payment is null)
			return Result.Fail($"Payment {command.PaymentId.Value} was not found.");
		
		INotification integrationEvent;

		var isStoreCreditEnough = await _storeCreditChecker
			.CheckIfStoreCreditIsEnoughAsync(payment.CustomerId, payment.TotalAmount, cancellationToken);
		if (!isStoreCreditEnough)
		{
			payment.Cancel(PaymentCancellationReason.CustomerReachedStoreCreditLimit);
			integrationEvent = new CustomerReachedStoreCreditLimit(payment.OrderId.Value);
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

		await _paymentWriteRepository
			.AppendEventsAndCommitAsync(payment, cancellationToken, integrationEvent);

		return Result.Ok();
	}
}
