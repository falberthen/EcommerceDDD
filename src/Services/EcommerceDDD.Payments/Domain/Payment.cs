using PaymentCompleted = EcommerceDDD.Payments.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.Payments.Domain;

public class Payment : AggregateRoot<PaymentId>
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
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

    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new BusinessRuleException($"Payment cannot be completed when '{Status}'");

        var @event = PaymentCompleted.Create(
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

    private void Apply(PaymentCreated created)
    {
        Status = PaymentStatus.Pending;
        Id = PaymentId.Of(created.PaymentId);
        CustomerId = CustomerId.Of(created.CustomerId);
        OrderId = OrderId.Of(created.OrderId);
        TotalAmount = Money.Of(
            created.TotalAmount,
            created.CurrencyCode);
        CreatedAt = created.CreatedAt;
    }

    private void Apply(PaymentCompleted completed)
    {
        Status = PaymentStatus.Completed;
        CompletedAt = completed.CompletedAt;
    }

    private void Apply(PaymentCanceled canceled)
    {
        Status = PaymentStatus.Canceled;
        CanceledAt = canceled.CanceledAt;
    }

    private Payment(PaymentData paymentData)
    {       
        var @event = PaymentCreated.Create(
            Guid.NewGuid(),
            paymentData.CustomerId.Value,
            paymentData.OrderId.Value,
            paymentData.TotalAmount.Amount,
            paymentData.TotalAmount.Currency.Code,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private Payment() {}
}
