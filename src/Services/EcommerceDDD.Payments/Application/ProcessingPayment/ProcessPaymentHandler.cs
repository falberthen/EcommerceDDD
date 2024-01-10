namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class ProcessPaymentHandler : ICommandHandler<ProcessPayment>
{
    private readonly ICustomerCreditChecker _creditChecker;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public ProcessPaymentHandler(
        ICustomerCreditChecker creditChecker,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _creditChecker = creditChecker;
        _paymentWriteRepository = paymentWriteRepository;        
    }

    public async Task Handle(ProcessPayment command, CancellationToken cancellationToken)
    {
        var payment = await _paymentWriteRepository
            .FetchStreamAsync(command.PaymentId.Value)
            ?? throw new RecordNotFoundException($"Payment {command.PaymentId} was not found.");

        try
        {
            if (!await _creditChecker.IsCreditEnough(payment.CustomerId, payment.TotalAmount))
                throw new BusinessRuleException($"Customer credit limit is not enough.");

            // Completing payment
            payment.Complete();

            // Persisting events
            _paymentWriteRepository.AppendIntegrationEvent(
                new PaymentCompleted(
                    payment.Id.Value,
                    payment.OrderId.Value,
                    payment.TotalAmount.Amount,
                    payment.TotalAmount.Currency.Code,
                    payment.CompletedAt!.Value));
            await _paymentWriteRepository.AppendEventsAsync(payment);
        }
        catch (BusinessRuleException) // Customer reached credit limit
        {
            payment.Cancel(PaymentCancellationReason.CustomerReachedCreditLimit);

            // Persisting events
            _paymentWriteRepository.AppendIntegrationEvent(
                new CustomerReachedCreditLimit(payment.OrderId.Value));
            await _paymentWriteRepository.AppendEventsAsync(payment);
        }
        catch (Exception) // Unexpected issue
        {
            payment.Cancel(PaymentCancellationReason.ProcessmentError);

            // Persisting events
            _paymentWriteRepository.AppendIntegrationEvent(
                new PaymentFailed(
                    payment.Id.Value,
                    payment.OrderId.Value,
                    payment.TotalAmount.Amount,
                    payment.TotalAmount.Currency.Code));
            await _paymentWriteRepository.AppendEventsAsync(payment);
        }
    }
}