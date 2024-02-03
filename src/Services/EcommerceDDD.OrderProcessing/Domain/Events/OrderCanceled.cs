namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record class OrderCanceled : DomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid? PaymentId { get; private set; }    
    public OrderCancellationReason OrderCancellationReason { get; private set; }
    public string OrderCancellationReasonDescription { get; private set; }

    public static OrderCanceled Create(
        Guid orderId,
        Guid? paymentId,
        OrderCancellationReason orderCancellationReason)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));

        return new OrderCanceled(
            orderId,
            paymentId,
            orderCancellationReason);
    }

    private OrderCanceled(
        Guid orderId,
        Guid? paymentId,
        OrderCancellationReason orderCancellationReason)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        OrderCancellationReason = orderCancellationReason;
        OrderCancellationReasonDescription = orderCancellationReason
            .GetDescription();

    }
}
