using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Orders.Application.Payments.RequestingPayment;

public class CustomerReachedCreditLimit : IIntegrationEvent
{
    public Guid OrderId { get; private set; }
    public DateTime CheckedAt { get; private set; }

    public CustomerReachedCreditLimit(Guid orderId)
    {
        OrderId = orderId;
        CheckedAt = DateTime.UtcNow;
    }
}