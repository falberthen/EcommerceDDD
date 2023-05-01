namespace EcommerceDDD.Orders.Application;

public class OrderSaga :
    IEventHandler<OrderPlaced>,
    IEventHandler<OrderPaid>,
    IEventHandler<PaymentCompleted>,
    IEventHandler<PackageShipped>
{
    private readonly ICommandBus _commandBus;

    public OrderSaga(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    public async Task Handle(OrderPlaced @event, CancellationToken cancellationToken)
    {
        var command = RequestPayment.Create(
            CustomerId.Of(@event.CustomerId),
            OrderId.Of(@event.OrderId),
            Money.Of(@event.TotalPrice, @event.CurrencyCode),
            Currency.OfCode(@event.CurrencyCode));

        await DelayOnPurpose(4000);
        await _commandBus.Send(command);
    }

    public async Task Handle(PaymentCompleted @event, CancellationToken cancellationToken)
    {
        var command = RecordPayment.Create(
            OrderId.Of(@event.OrderId),
            PaymentId.Of(@event.PaymentId),
            Money.Of(@event.TotalAmount, @event.CurrencyCode));

        await DelayOnPurpose(4000);
        await _commandBus.Send(command);
    }

    public async Task Handle(OrderPaid @event, CancellationToken cancellationToken)
    {
        var command = RequestShipment.Create(
            OrderId.Of(@event.OrderId));

        await DelayOnPurpose(4000);
        await _commandBus.Send(command);
    }

    public async Task Handle(PackageShipped @event, CancellationToken cancellationToken)
    {
        var command = CompleteOrder.Create(
            OrderId.Of(@event.OrderId),
            ShipmentId.Of(@event.ShipmentId));

        await DelayOnPurpose(4000);
        await _commandBus.Send(command);
    }

    // Delaying just to allow you to see the statuses changing on the UI =)
    private Task DelayOnPurpose(int milliseconts) =>
        Task.Delay(milliseconts);
}