using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Application.CompletingPayment;

public class ProcessPaymentHandler : INotificationHandler<PaymentRequested>
{
    private readonly IMediator _mediator;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public ProcessPaymentHandler(
        IMediator mediator,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _mediator = mediator;
        _paymentWriteRepository = paymentWriteRepository;
    }

    public async Task Handle(PaymentRequested @event, CancellationToken cancellationToken)
    {
        // Pretenting processing the payment with an external provider...
        // 1..2..3

        var payment = await _paymentWriteRepository
            .FetchStream(@event.PaymentId.Value);

        if (payment == null)
            throw new ApplicationException($"Cannot find payment {@event.PaymentId}.");

        // Recording payment processement
        var paymentProcessedEvent = payment.RecordProcessement();

        await _paymentWriteRepository
            .AppendEventsAsync(payment);

        await _mediator.Publish(paymentProcessedEvent);
    }
}