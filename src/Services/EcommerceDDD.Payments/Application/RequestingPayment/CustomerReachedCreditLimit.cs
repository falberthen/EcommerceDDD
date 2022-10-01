using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

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