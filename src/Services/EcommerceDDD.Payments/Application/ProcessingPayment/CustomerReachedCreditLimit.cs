namespace EcommerceDDD.Payments.Application.ProcessingPayment;

public class CustomerReachedCreditLimit : IntegrationEvent
{
    public Guid OrderId { get; private set; }
    public DateTime CheckedAt { get; private set; }

    public CustomerReachedCreditLimit(Guid orderId)
    {
        OrderId = orderId;
        CheckedAt = DateTime.UtcNow;
    }
}