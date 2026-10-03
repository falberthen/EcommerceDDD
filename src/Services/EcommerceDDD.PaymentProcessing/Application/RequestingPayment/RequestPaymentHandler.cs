namespace EcommerceDDD.PaymentProcessing.Application.RequestingPayment;

public class RequestPaymentHandler(
	IOrderPaymentLookup orderPaymentLookup,
	IEventStoreRepository<Payment> paymentWriteRepository
)
{
	private readonly IOrderPaymentLookup _orderPaymentLookup = orderPaymentLookup;
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(RequestPayment command, CancellationToken cancellationToken)
    {
		// Requested by an earlier attempt.
		if (await _orderPaymentLookup.HasPaymentAsync(command.OrderId, cancellationToken))
			return Result.Ok();

        var paymentData = new PaymentData(
            command.CustomerId,
            command.OrderId,
            command.TotalAmount,
			command.ProductItems);

        var payment = Payment.Create(paymentData);

		var paymentCreatedEvent = payment.GetUncommittedEvents()
			.OfType<PaymentCreated>()
			.FirstOrDefault();

		// Committed with the payment, so the payment is guaranteed to be processed.
        await _paymentWriteRepository
			.AppendEventsAndCommitAsync(payment, cancellationToken, paymentCreatedEvent!);

        return Result.Ok();
    }
}
