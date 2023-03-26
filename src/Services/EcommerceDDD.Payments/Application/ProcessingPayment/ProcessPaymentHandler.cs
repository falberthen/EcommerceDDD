using MediatR;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class ProcessPaymentHandler : ICommandHandler<ProcessPayment>
{
    private readonly ICustomerCreditChecker _creditChecker;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;
    private readonly IOutboxMessageService _outboxMessageService;

    public ProcessPaymentHandler(
        ICustomerCreditChecker creditChecker,
        IEventStoreRepository<Payment> paymentWriteRepository,
        IOutboxMessageService outboxMessageService)
    {
        _creditChecker = creditChecker;
        _paymentWriteRepository = paymentWriteRepository;
        _outboxMessageService = outboxMessageService;
    }

    public async Task Handle(ProcessPayment command, CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
            .FetchStreamAsync(command.PaymentId.Value);

        try
        {
            if (!await _creditChecker.IsCreditEnough(payment.CustomerId, payment.TotalAmount))
                throw new BusinessRuleException($"Customer credit limit is not enough.");

            // Completing payment
            payment.Complete();

            await _outboxMessageService
                .SaveAsOutboxMessageAsync(new PaymentCompleted(
                    payment.Id.Value,
                    payment.OrderId.Value,
                    payment.TotalAmount.Amount,
                    payment.TotalAmount.Currency.Code,
                    payment.CompletedAt!.Value));
        }
        catch (BusinessRuleException)
        {
            payment.Cancel(PaymentCancellationReason.CustomerReachedCreditLimit);
            await _outboxMessageService
                .SaveAsOutboxMessageAsync(new CustomerReachedCreditLimit(
                    payment.OrderId.Value));
        }
        catch (Exception)
        {
            // unexpected issue
            payment.Cancel(PaymentCancellationReason.ProcessmentError);
            await _outboxMessageService
                .SaveAsOutboxMessageAsync(
                    new PaymentFailed(
                        payment.Id.Value,
                        payment.OrderId.Value,
                        payment.TotalAmount.Amount,
                        payment.TotalAmount.Currency.Code));
        }

        await _paymentWriteRepository
            .AppendEventsAsync(payment);
    }
}

public record class CreditLimitModel(Guid CustomerId, decimal CreditLimit);