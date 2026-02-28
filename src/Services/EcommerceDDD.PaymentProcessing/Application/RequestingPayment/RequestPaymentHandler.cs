namespace EcommerceDDD.PaymentProcessing.Application.RequestingPayment;

public class RequestPaymentHandler(
	ICommandBus commandBus,
	IEventStoreRepository<Payment> paymentWriteRepository
) : ICommandHandler<RequestPayment>
{
	private readonly ICommandBus _commandBus = commandBus;
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(RequestPayment command, CancellationToken cancellationToken)
    {
        Activity.Current?.SetTag("order.id", command.OrderId.Value.ToString());
        var paymentData = new PaymentData(
            command.CustomerId,
            command.OrderId,
            command.TotalAmount,
			command.ProductItems);

        var payment = Payment.Create(paymentData);

        await _paymentWriteRepository
			.AppendEventsAsync(payment, cancellationToken);

        return await _commandBus.SendAsync(
            ProcessPayment.Create(payment.Id), cancellationToken);
    }
}
