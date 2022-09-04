using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Application.FinalizingPayment;

public class PaymentProcessedHandler : INotificationHandler<DomainEventNotification<PaymentProcessed>>
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProcessedHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(DomainEventNotification<PaymentProcessed> notification, CancellationToken cancellationToken)
    {
        using var scopedService = _serviceProvider.CreateScope();
        var paymentWriteRepository = scopedService
           .ServiceProvider.GetRequiredService<IEventStoreRepository<Payment>>();
        var eventProducer = scopedService
           .ServiceProvider.GetRequiredService<IEventProducer>();

        var @event = notification.DomainEvent;

        var payment = await paymentWriteRepository
            .FetchStream(@event.PaymentId.Value);

        if (payment == null)
            throw new ApplicationException($"Cannot find payment {@event.PaymentId}.");

        // Notifying Order Saga
        await eventProducer
            .PublishAsync(new PaymentFinalized(
                payment.Id.Value,
                payment.OrderId.Value,
                payment.TotalAmount.Value,
                payment.TotalAmount.CurrencyCode),
                cancellationToken);
    }
}