namespace EcommerceDDD.OrderProcessing.Application.Payments.ProcessingPayment.IntegrationEvents;

public class PaymentFailed(
    Guid paymentId,
    Guid orderId,
    decimal totalAmount,
    string currencyCode) : IntegrationEvent
{
    public Guid PaymentId { get; } = paymentId;
    public Guid OrderId { get; } = orderId;
    public decimal TotalAmount { get; } = totalAmount;
    public string CurrencyCode { get; } = currencyCode;
    public DateTime FailedAt { get; } = DateTime.UtcNow;
}
