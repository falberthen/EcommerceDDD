﻿namespace EcommerceDDD.OrderProcessing.Application.Orders.CancelingOrder;

public class CancelOrderHandler(
    IOrderStatusBroadcaster orderStatusBroadcaster,
    IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<CancelOrder>
{
	private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;

	public async Task HandleAsync(CancelOrder command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken) 
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Canceling order
        order.Cancel(command.CancellationReason);
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