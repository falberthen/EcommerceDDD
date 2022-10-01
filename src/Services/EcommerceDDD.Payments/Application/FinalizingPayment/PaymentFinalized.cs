using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Payments.Application.FinalizingPayment;

public class PaymentFinalized : IIntegrationEvent
{
    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public string CurrencyCode { get; }
    public DateTime FinalizedAt { get; }

    public PaymentFinalized(
        Guid paymentId, 
        Guid orderId, 
        decimal totalAmount, 
        string currencyCode)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        FinalizedAt = DateTime.UtcNow;
    }
}
