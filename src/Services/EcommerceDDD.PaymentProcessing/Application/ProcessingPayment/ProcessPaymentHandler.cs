namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment;

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
        await Task.Delay(TimeSpan.FromSeconds(3)); // 3-second delay

        var payment = await _paymentWriteRepository
            .FetchStreamAsync(command.PaymentId.Value)
            ?? throw new RecordNotFoundException($"Payment {command.PaymentId} was not found.");

        try
        {
            if (!await _creditChecker.IsCreditEnough(payment.CustomerId, payment.TotalAmount))
                throw new BusinessRuleException($"Customer credit limit is not enough.");

            // Completing payment
            payment.Complete();

            // Appending to outbox for message broker
            _paymentWriteRepository.AppendToOutbox(
               new PaymentFinalized(
                   payment.Id.Value,
                   payment.OrderId.Value,
                   payment.TotalAmount.Amount,
                   payment.TotalAmount.Currency.Code,
                   payment.CompletedAt!.Value));

            // Persisting aggregate
            await _paymentWriteRepository
                .AppendEventsAsync(payment);
        }
        catch (BusinessRuleException) // Customer reached credit limit
        {
            payment.Cancel(PaymentCancellationReason.CustomerReachedCreditLimit);

            // Appending integration event to outbox
            _paymentWriteRepository.AppendToOutbox(
                new CustomerReachedCreditLimit(payment.OrderId.Value));

            // Persisting domain event
            await _paymentWriteRepository.AppendEventsAsync(payment);
        }
        catch (Exception) // Unexpected issue
        {
            payment.Cancel(PaymentCancellationReason.ProcessmentError);

            // Appending integration event to outbox
            _paymentWriteRepository.AppendToOutbox(
                new PaymentFailed(
                    payment.Id.Value,
                    payment.OrderId.Value,
                    payment.TotalAmount.Amount,
                    payment.TotalAmount.Currency.Code));

            // Persisting domain event
            await _paymentWriteRepository.AppendEventsAsync(payment);
        }
    }
}