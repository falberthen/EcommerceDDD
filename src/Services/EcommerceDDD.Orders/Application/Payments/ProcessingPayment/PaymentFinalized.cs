using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Orders.Application.Payments.ProcessingPayment;

public class PaymentCompleted : IntegrationEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime CompletedAt { get; set; }
}
