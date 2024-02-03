using PaymentCompleted = EcommerceDDD.PaymentProcessing.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.PaymentProcessing.Infrastructure.Projections;

public class PaymentDetails
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; }

    internal void Apply(PaymentCreated requested)
    {
        Id = requested.PaymentId;
        CustomerId = requested.CustomerId;
        OrderId = requested.OrderId;
        TotalAmount = requested.TotalAmount;
        CurrencyCode = requested.CurrencyCode;
        Status = PaymentStatus.Pending;
    }

    internal void Apply(PaymentCompleted @event)
    {
        CompletedAt = @event.Timestamp;
        Status = PaymentStatus.Completed;
    }

    internal void Apply(PaymentCanceled @event)
    {
        CanceledAt = @event.Timestamp;
        Status = PaymentStatus.Canceled;
    }
}