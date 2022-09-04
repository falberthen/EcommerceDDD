using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class PaymentRequestedHandler : INotificationHandler<DomainEventNotification<PaymentRequested>>
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentRequestedHandler(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(DomainEventNotification<PaymentRequested> notification, CancellationToken cancellationToken)
    {
        using var scopedService = _serviceProvider.CreateScope();
        var paymentWriteRepository = scopedService
           .ServiceProvider.GetRequiredService<IEventStoreRepository<Payment>>();

        // Pretending it is processing the payment with an external provider...
        // 1..2..3

        var @event = notification.DomainEvent;

        var payment = await paymentWriteRepository
            .FetchStream(@event.PaymentId.Value);

        if (payment == null)
            throw new ApplicationException($"Cannot find payment {@event.PaymentId}.");

        // Recording payment processement
        payment.RecordProcessement();

        await paymentWriteRepository
            .AppendEventsAsync(payment);
    }
}