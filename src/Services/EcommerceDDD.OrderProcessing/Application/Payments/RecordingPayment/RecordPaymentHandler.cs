namespace EcommerceDDD.OrderProcessing.Application.Payments.RecordingPayment;

public class RecordPaymentHandler(
    IOrderStatusBroadcaster orderStatusBroadcaster,
    IEventStoreRepository<Order> orderWriteRepository) : ICommandHandler<RecordPayment>
{
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster = orderStatusBroadcaster;
    private readonly IEventStoreRepository<Order> _orderWriteRepository = orderWriteRepository;

    public async Task HandleAsync(RecordPayment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Recording the payment
        order.RecordPayment(command.PaymentId, command.TotalPaid);

        // Persisting aggregate
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