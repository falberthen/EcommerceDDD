using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Domain;

public class Payment : AggregateRoot<PaymentId>
{
    public OrderId OrderId { get; private set; }
    public Money TotalAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public static Payment CreateNew(OrderId orderId, Money totalAmount)
    {
        if (orderId == null)
            throw new DomainException("The order id is required.");
    
        if (totalAmount == null)
            throw new DomainException("The total amount is required.");

        return new Payment(orderId, totalAmount);
    }

    public void RecordProcessement()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Payment cannot be processed when '{Status}'");

        var @event = PaymentProcessed.Create(Id, DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(PaymentRequested requested)
    {
        Status = PaymentStatus.Pending;
        Id = requested.PaymentId;
        OrderId = requested.OrderId;
        TotalAmount = requested.TotalAmount;        
    }

    private void Apply(PaymentProcessed processed)
    {
        Status = PaymentStatus.Processed;
        ProcessedAt = processed.ProcessedAt;
    }

    private Payment(OrderId orderId, Money totalAmount)
    {       
        var @event = PaymentRequested.Create(
            PaymentId.Of(Guid.NewGuid()),
            orderId,
            totalAmount);

        AppendEvent(@event);
        Apply(@event);
    }

    private Payment() { }
}
