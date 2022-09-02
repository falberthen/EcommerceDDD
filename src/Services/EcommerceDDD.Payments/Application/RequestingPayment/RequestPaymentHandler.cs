using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

public class RequestPaymentHandler : CommandHandler<RequestPayment>
{
    private readonly IMediator _mediator;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public RequestPaymentHandler(
        IMediator mediator,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _mediator = mediator;
        _paymentWriteRepository = paymentWriteRepository;
    }

    public override async Task Handle(RequestPayment command,
        CancellationToken cancellationToken)
    {
        var payment = Payment.CreateNew(command.OrderId, command.TotalAmount);

        var @event = payment.GetUncommittedEvents()
            .FirstOrDefault(e => e.GetType() == typeof(PaymentRequested));

        await _paymentWriteRepository
            .AppendEventsAsync(payment, cancellationToken);
       
        await _mediator.Publish(@event);
    }
}