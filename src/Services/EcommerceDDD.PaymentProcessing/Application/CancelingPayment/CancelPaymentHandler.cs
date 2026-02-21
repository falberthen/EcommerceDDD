namespace EcommerceDDD.PaymentProcessing.Application.CancelingPayment;

public class CancelPaymentHandler(IEventStoreRepository<Payment> paymentWriteRepository) : ICommandHandler<CancelPayment>
{
	private readonly IEventStoreRepository<Payment> _paymentWriteRepository = paymentWriteRepository;

	public async Task<Result> HandleAsync(CancelPayment command, CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
			.FetchStreamAsync(command.PaymentId.Value, cancellationToken: cancellationToken);

        if (payment is null)
            return Result.Fail($"Failed to find the payment {command.PaymentId}.");

        // Canceling payment
        payment.Cancel(command.PaymentCancellationReason);
        await _paymentWriteRepository
			.AppendEventsAsync(payment, cancellationToken);

        return Result.Ok();
    }
}
