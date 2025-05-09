﻿namespace EcommerceDDD.OrderProcessing.Application.Shipments.RecordingShipment;

public class RecordShipmentHandler(
    IOrderStatusBroadcaster orderStatusBroadcaster,
    IEventStoreRepository<Order> orderWriteRepository
) : ICommandHandler<RecordShipment>
{
	private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
	private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;

	public async Task HandleAsync(RecordShipment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
			.FetchStreamAsync(command.OrderId.Value, cancellationToken: cancellationToken)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Recording shipment
        order.RecordShipment(command.ShipmentId);
        await _orderWriteRepository
			.AppendEventsAsync(order, cancellationToken);

        // Updating order status on the UI with SignalR
        await _orderStatusBroadcaster.UpdateOrderStatus(
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}