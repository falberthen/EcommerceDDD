using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class PaymentFailed : IntegrationEvent
{
    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public string CurrencyCode { get; }
    public DateTime FailedAt { get; }

    public PaymentFailed(
        Guid paymentId, 
        Guid orderId, 
        decimal totalAmount, 
        string currencyCode)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        FailedAt = DateTime.UtcNow;
    }
}
