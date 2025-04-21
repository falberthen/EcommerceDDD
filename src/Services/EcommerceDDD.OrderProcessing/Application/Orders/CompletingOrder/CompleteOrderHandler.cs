namespace EcommerceDDD.OrderProcessing.Application.Orders.CompletingOrder;

public class CompleteOrderHandler(
    IOrderStatusBroadcaster orderStatusBroadcaster,
    IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<CompleteOrder>
{
	private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;

	public async Task HandleAsync(CompleteOrder command, CancellationToken cancellationToken)
    {
		await Task.Delay(TimeSpan.FromSeconds(3)); // 3-second delay

		var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Completing order
        order.Complete(command.ShipmentId);

        await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken: cancellationToken);
        
        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}