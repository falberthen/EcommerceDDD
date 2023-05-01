namespace EcommerceDDD.Payments.Application.CancelingPayment;

public class CancelPaymentHandler : ICommandHandler<CancelPayment>
{
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public CancelPaymentHandler(
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _paymentWriteRepository = paymentWriteRepository;
    }

    public async Task Handle(CancelPayment command,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
            .FetchStreamAsync(command.PaymentId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the payment {command.PaymentId}.");

        // Canceling payment
        payment.Cancel(command.PaymentCancellationReason);
        await _paymentWriteRepository
            .AppendEventsAsync(payment);
    }
}