namespace EcommerceDDD.Orders.Domain.Commands;

public record class CancelOrder : ICommand
{
    public OrderId OrderId { get; private set; }
    public OrderCancellationReason CancellationReason { get; private set; }

    public static CancelOrder Create(
       OrderId orderId,
       OrderCancellationReason CancellationReason)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));

        return new CancelOrder(orderId, CancellationReason);
    }

    private CancelOrder(
        OrderId orderId,
        OrderCancellationReason cancellationReason)
    {
        OrderId = orderId;
        CancellationReason = cancellationReason;
    }
}