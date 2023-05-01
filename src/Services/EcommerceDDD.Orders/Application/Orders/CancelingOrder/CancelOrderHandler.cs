namespace EcommerceDDD.Orders.Application.Orders.CancelingOrder;

public class CancelOrderHandler : ICommandHandler<CancelOrder>
{
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;

    public CancelOrderHandler(
        IOrderStatusBroadcaster orderStatusBroadcaster,
        IEventStoreRepository<Order> orderWriteRepository)
    {
        _orderStatusBroadcaster = orderStatusBroadcaster;
        _orderWriteRepository = orderWriteRepository;
    }

    public async Task Handle(CancelOrder command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value) 
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Canceling order
        order.Cancel(command.CancellationReason);
        await _orderWriteRepository
            .AppendEventsAsync(order);
        
        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}