using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Orders.Application.Payments.FinalizingPayment;

public class PaymentFinalized : IIntegrationEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime FinalizedAt { get; set; }
}
