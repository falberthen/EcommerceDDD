using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Application.FinalizingPayment;

public class PaymentProcessedHandler : INotificationHandler<PaymentProcessed>
{
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;
    private readonly IEventProducer _eventProducer;

    public PaymentProcessedHandler(
        IEventStoreRepository<Payment> paymentWriteRepository,
        IEventProducer eventProducer)
    {
        _paymentWriteRepository = paymentWriteRepository;
        _eventProducer = eventProducer;
    }

    public async Task Handle(PaymentProcessed @event, CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
            .FetchStream(@event.PaymentId.Value);

        if (payment == null)
            throw new ApplicationException($"Cannot find payment {@event.PaymentId}.");

        // Notifying Order Saga
        await _eventProducer
            .PublishAsync(new PaymentFinalized(
                payment.Id.Value,
                payment.OrderId.Value,
                payment.TotalAmount.Value,
                payment.TotalAmount.CurrencyCode),
                cancellationToken);
    }
}