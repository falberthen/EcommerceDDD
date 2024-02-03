namespace EcommerceDDD.OrderProcessing.Application.Payments.RecordingPayment;

public class RecordPaymentHandler : ICommandHandler<RecordPayment>
{
    private readonly IOrderStatusBroadcaster _orderStatusBroadcaster;
    private readonly IEventStoreRepository<Order> _orderWriteRepository;

    public RecordPaymentHandler(
        IOrderStatusBroadcaster orderStatusBroadcaster,
        IEventStoreRepository<Order> orderWriteRepository)
    {
        _orderStatusBroadcaster = orderStatusBroadcaster;
        _orderWriteRepository = orderWriteRepository;        
    }

    public async Task Handle(RecordPayment command, CancellationToken cancellationToken)
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