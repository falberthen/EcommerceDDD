using MediatR;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

public class RequestPaymentHandler : ICommandHandler<RequestPayment>
{
    private readonly IEventProducer _eventProducer;
    private readonly ICustomerCreditChecker _creditChecker;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public RequestPaymentHandler(
        IEventProducer eventProducer,
        ICustomerCreditChecker creditChecker,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _eventProducer = eventProducer;
        _creditChecker = creditChecker;
        _paymentWriteRepository = paymentWriteRepository;
    }

    public async Task<Unit> Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        var paymentData = new PaymentData(
            command.CustomerId, 
            command.OrderId, 
            command.TotalAmount);

        if(!await _creditChecker.EnsureEnoughCredit(command.CustomerId, command.TotalAmount))
        {
            await _eventProducer
                .PublishAsync(new CustomerReachedCreditLimit(command.OrderId.Value), cancellationToken);
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
