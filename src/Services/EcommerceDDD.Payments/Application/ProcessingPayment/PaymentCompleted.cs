using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class PaymentCompleted : IntegrationEvent
{
    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public string CurrencyCode { get; }
    public DateTime CompletedAt { get; }

    public PaymentCompleted(
        Guid paymentId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode,
        DateTime completedAt)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        CompletedAt = completedAt;
    }
}
