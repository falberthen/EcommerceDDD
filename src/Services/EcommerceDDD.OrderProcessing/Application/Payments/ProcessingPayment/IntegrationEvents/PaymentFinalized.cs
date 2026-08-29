namespace EcommerceDDD.OrderProcessing.Application.Payments.ProcessingPayment.IntegrationEvents;

[MessageIdentity(nameof(PaymentFinalized))]
public class PaymentFinalized : IntegrationEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public required string CurrencyCode { get; set; }
    public DateTime CompletedAt { get; set; }
}
