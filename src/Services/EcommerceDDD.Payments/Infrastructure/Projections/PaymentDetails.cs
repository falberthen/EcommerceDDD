using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Payments.Domain.Events;

namespace EcommerceDDD.Payments.Infrastructure.Projections;

public class PaymentDetails
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; }

    public void Apply(PaymentRequested requested)
    {
        Id = requested.PaymentId;
        CustomerId = requested.CustomerId;
        OrderId = requested.OrderId;
        TotalAmount = requested.TotalAmount;
        CurrencyCode = requested.CurrencyCode;
        Status = PaymentStatus.Pending;
    }

    public void Apply(PaymentProcessed processed)
    {
        ProcessedAt = processed.ProcessedAt;
        Status = PaymentStatus.Processed;
    }

    public void Apply(PaymentCanceled cancelled)
    {
        CanceledAt = cancelled.CanceledAt;
        Status = PaymentStatus.Canceled;
    }
}