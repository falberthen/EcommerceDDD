using MediatR;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

public class RequestPaymentHandler : ICommandHandler<RequestPayment>
{
    private readonly ICustomerCreditChecker _creditChecker;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;
    private readonly IOutboxMessageService _outboxMessageService;

    public RequestPaymentHandler(
        ICustomerCreditChecker creditChecker,
        IEventStoreRepository<Payment> paymentWriteRepository,
        IOutboxMessageService outboxMessageService)
    {
        _creditChecker = creditChecker;
        _paymentWriteRepository = paymentWriteRepository;
        _outboxMessageService = outboxMessageService;
    }

    public async Task<Unit> Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        var paymentData = new PaymentData(
            command.CustomerId, 
            command.OrderId, 
            command.TotalAmount);

        if(!await _creditChecker.EnsureEnoughCredit(command.CustomerId, command.TotalAmount))
        {
            await _outboxMessageService.SaveAsOutboxMessageAsync(new CustomerReachedCreditLimit(command.OrderId.Value));
            throw new BusinessRuleException($"Customer credit limit is not enough.");
        }
        
        var payment = Payment.Create(paymentData);

        // Recording payment processement
        payment.RecordProcessement();
        await _paymentWriteRepository
            .AppendEventsAsync(payment);

        return Unit.Value;
    }
}

public record class CreditLimitModel(Guid CustomerId, decimal CreditLimit);
