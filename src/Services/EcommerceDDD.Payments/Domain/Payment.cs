using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Domain;

public class Payment : AggregateRoot<PaymentId>
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
    public Money TotalAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public static Payment CreateNew(CustomerId customerId, OrderId orderId, Money totalAmount)
    {
        if (customerId == null)
            throw new DomainException("The customer Id is required.");

        if (orderId == null)
            throw new DomainException("The order id is required.");
    
        if (totalAmount == null)
            throw new DomainException("The total amount is required.");

        return new Payment(customerId, orderId, totalAmount);
    }

    public void RecordProcessement()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Payment cannot be processed when '{Status}'");

        var @event = PaymentProcessed.Create(Id, DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel(PaymentCancellationReason PaymentCancellationReason)
    {
        if (Status == PaymentStatus.Canceled)
            throw new DomainException($"Payment cannot be canceled when '{Status}'");

        var @event = PaymentCanceled.Create(Id, PaymentCancellationReason);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(PaymentRequested requested)
    {
        Status = PaymentStatus.Pending;
        Id = requested.PaymentId;
        CustomerId = requested.CustomerId;
        OrderId = requested.OrderId;
        TotalAmount = requested.TotalAmount;        
    }

    private void Apply(PaymentProcessed processed)
    {
        Status = PaymentStatus.Processed;
        ProcessedAt = processed.ProcessedAt;
    }

    private void Apply(PaymentCanceled canceled)
    {
        Status = PaymentStatus.Canceled;
    }

    private Payment(CustomerId customerId, OrderId orderId, Money totalAmount)
    {       
        var @event = PaymentRequested.Create(
            customerId,
            PaymentId.Of(Guid.NewGuid()),
            orderId,
            totalAmount);

        AppendEvent(@event);
        Apply(@event);
    }

    private Payment() { }
}
