using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.IntegrationServices;
using EcommerceDDD.Payments.Domain;

namespace EcommerceDDD.Payments.Application.CancelingPayment;

public class CancelPaymentHandler : CommandHandler<CancelPayment>
{
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public CancelPaymentHandler(
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _paymentWriteRepository = paymentWriteRepository;
    }

    public override async Task Handle(CancelPayment command,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
            .FetchStream(command.PaymentId.Value);

        if (payment == null)
            throw new ApplicationException($"Failed to find the payment {command.PaymentId}.");

        // Canceling payment
        payment.Cancel(command.PaymentCancellationReason);
        await _paymentWriteRepository
            .AppendEventsAsync(payment);
    }
}