using MediatR;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Payments.Application.FinalizingPayment;

public class PaymentProcessedHandler : INotificationHandler<DomainNotification<PaymentProcessed>>
{
    private readonly IEventProducer _eventProducer;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public PaymentProcessedHandler(
        IEventProducer eventProducer,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _eventProducer = eventProducer;
        _paymentWriteRepository = paymentWriteRepository;
    }

    public async Task Handle(DomainNotification<PaymentProcessed> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;

        var payment = await _paymentWriteRepository
            .FetchStreamAsync(@event.PaymentId)
            ?? throw new ApplicationLogicException($"Cannot find payment {@event.PaymentId}.");

        // Notifying Order Saga
        await _eventProducer
            .PublishAsync(new PaymentFinalized(
                payment.Id.Value,
                payment.OrderId.Value,
                payment.TotalAmount.Amount,
                payment.TotalAmount.Currency.Code),
                cancellationToken);
    }
}