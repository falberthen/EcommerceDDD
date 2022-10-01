using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Domain;

public class Payment : AggregateRoot<PaymentId>
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public Money TotalAmount { get; private set; }

    public static Payment Create(PaymentData paymentData)
    {
        var (CustomerId, OrderId, TotalAmount) = paymentData
            ?? throw new ArgumentNullException(nameof(paymentData));

        if (CustomerId is null)
            throw new BusinessRuleException("The customer Id is required.");

        if (OrderId is null)
            throw new BusinessRuleException("The order Id is required.");
    
        if (TotalAmount is null)
            throw new BusinessRuleException("The total amount is required.");

        return new Payment(paymentData);
    }

    public void RecordProcessement()
    {
        if (Status != PaymentStatus.Pending)
            throw new BusinessRuleException($"Payment cannot be processed when '{Status}'");

        var @event = PaymentProcessed.Create(
            Id.Value,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel(PaymentCancellationReason PaymentCancellationReason)
    {
        if (Status == PaymentStatus.Canceled)
            throw new BusinessRuleException($"Payment cannot be canceled when '{Status}'");

        var @event = PaymentCanceled.Create(
            Id.Value,            
            DateTime.UtcNow,
            PaymentCancellationReason);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(PaymentRequested requested)
    {
        Status = PaymentStatus.Pending;
        Id = PaymentId.Of(requested.PaymentId);
        CustomerId = CustomerId.Of(requested.CustomerId);
        OrderId = OrderId.Of(requested.OrderId);
        TotalAmount = Money.Of(
            requested.TotalAmount, 
            requested.CurrencyCode);
    }

    private void Apply(PaymentProcessed processed)
    {
        Status = PaymentStatus.Processed;
        ProcessedAt = processed.ProcessedAt;
    }

    private void Apply(PaymentCanceled canceled)
    {
        Status = PaymentStatus.Canceled;
        CanceledAt = canceled.CanceledAt;
    }

    private Payment(PaymentData paymentData)
    {       
        var @event = PaymentRequested.Create(
            Guid.NewGuid(),
            paymentData.CustomerId.Value,
            paymentData.OrderId.Value,
            paymentData.TotalAmount.Amount,
            paymentData.TotalAmount.Currency.Code);

        AppendEvent(@event);
        Apply(@event);
    }

    private Payment() {}
}
