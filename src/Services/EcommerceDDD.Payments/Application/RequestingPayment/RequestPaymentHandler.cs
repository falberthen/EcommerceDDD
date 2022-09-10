using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

public class RequestPaymentHandler : CommandHandler<RequestPayment>
{
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public RequestPaymentHandler(
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _paymentWriteRepository = paymentWriteRepository;
    }

    public override async Task Handle(RequestPayment command,
        CancellationToken cancellationToken)
    {
        var payment = Payment.CreateNew(
            command.CustomerId, 
            command.OrderId, 
            command.TotalAmount);

        await _paymentWriteRepository
            .AppendEventsAsync(payment, cancellationToken);       
    }
}