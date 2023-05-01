namespace EcommerceDDD.Orders.Application.Orders.CompletingOrder;

public class CompleteOrderHandler : ICommandHandler<CompleteOrder>
{
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;

    public CompleteOrderHandler(
        IOrderStatusBroadcaster orderStatusBroadcaster,
        IEventStoreRepository<Order> orderWriteRepository)
    {
        _orderStatusBroadcaster = orderStatusBroadcaster;
        _orderWriteRepository = orderWriteRepository;
    }

    public async Task Handle(CompleteOrder command, CancellationToken cancellationToken)
    {       
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Completing order
        order.Complete(command.ShipmentId);

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