namespace EcommerceDDD.PaymentProcessing.Application.ProcessingPayment.IntegrationEvents;

[MessageIdentity(nameof(CustomerReachedStoreCreditLimit))]
public class CustomerReachedStoreCreditLimit : IntegrationEvent
{
    public Guid OrderId { get; private set; }
    public DateTime CheckedAt { get; private set; }

    public CustomerReachedStoreCreditLimit(Guid orderId)
    {
        OrderId = orderId;
        CheckedAt = DateTime.UtcNow;
    }
}