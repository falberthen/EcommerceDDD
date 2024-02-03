namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment.IntegrationEvents;

public class PaymentFinalized : IntegrationEvent
{
    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    public string CurrencyCode { get; }
    public DateTime CompletedAt { get; }

    public PaymentFinalized(
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
