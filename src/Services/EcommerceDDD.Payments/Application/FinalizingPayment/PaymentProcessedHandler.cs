using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Payments.Domain.Events;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Payments.Application.FinalizingPayment;

public class PaymentProcessedHandler : INotificationHandler<PaymentProcessed>
{
    private readonly IOutboxMessageService _outboxMessageService;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public PaymentProcessedHandler(
        IOutboxMessageService outboxMessageService,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _outboxMessageService = outboxMessageService;
        _paymentWriteRepository = paymentWriteRepository;
    }

    public async Task Handle(PaymentProcessed @event, CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
            .FetchStreamAsync(@event.PaymentId)
            ?? throw new ApplicationLogicException($"Cannot find payment {@event.PaymentId}.");

        await _outboxMessageService.SaveAsOutboxMessageAsync(
           new PaymentFinalized(
                payment.Id.Value,
                payment.OrderId.Value,
                payment.TotalAmount.Amount,
                payment.TotalAmount.Currency.Code)
       );
    }
}