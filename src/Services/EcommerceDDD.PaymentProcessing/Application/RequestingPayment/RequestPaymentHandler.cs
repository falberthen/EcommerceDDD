namespace EcommerceDDD.PaymentProcessing.Application.RequestingPayment;

public class RequestPaymentHandler(
	IMessageBus bus,
	IEventStoreRepository<Payment> paymentWriteRepository
)
{
	private readonly IMessageBus _bus = bus;
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(RequestPayment command, CancellationToken cancellationToken)
    {
        var paymentData = new PaymentData(
            command.CustomerId,
            command.OrderId,
            command.TotalAmount,
			command.ProductItems);

        var payment = Payment.Create(paymentData);

        await _paymentWriteRepository
			.AppendEventsAndCommitAsync(payment, cancellationToken);

        return await _bus.InvokeAsync<Result>(
            ProcessPayment.Create(payment.Id, command.OrderId), cancellationToken);
    }
}
