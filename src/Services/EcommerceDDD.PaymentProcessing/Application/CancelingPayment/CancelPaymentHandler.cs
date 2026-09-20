namespace EcommerceDDD.PaymentProcessing.Application.CancelingPayment;

public class CancelPaymentHandler(
	IEventStoreRepository<Payment> paymentWriteRepository,
	IProductInventoryHandler productInventoryHandler)
{
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;
	private readonly IProductInventoryHandler _productInventoryHandler = productInventoryHandler;

	public async Task<Result> HandleAsync(CancelPayment command, CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
			.FetchForWritingAsync(command.PaymentId.Value, cancellationToken: cancellationToken);

        if (payment is null)
            return Result.Fail($"Failed to find the payment {command.PaymentId}.");

        // Stock is only decremented when a payment completes, so only a completed payment
        // Product is put back when the order is later canceled (e.g. undeliverable shipment).
        var shouldRestock = payment.Status == PaymentStatus.Completed;

        payment.Cancel(command.PaymentCancellationReason);
        await _paymentWriteRepository
			.AppendEventsAndCommitAsync(payment, cancellationToken);

        if (shouldRestock)
            await _productInventoryHandler
				.IncreaseQuantityInStockAsync(payment.ProductItems, cancellationToken);

        return Result.Ok();
    }
}
