namespace EcommerceDDD.Orders.Application;

/// <summary>
/// Handles compensation events for OrderSaga
/// </summary>
public class OrderSagaCompensation :
    IEventHandler<PaymentFailed>,
    IEventHandler<CustomerReachedCreditLimit>,
    IEventHandler<ProductWasOutOfStock>,
    IEventHandler<OrderCanceled>
{
    private readonly ICommandBus _commandBus;

    public OrderSagaCompensation(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    public Task Handle(PaymentFailed @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.PaymentFailed);

        return _commandBus.Send(command);
    }

    public async Task Handle(CustomerReachedCreditLimit @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.CustomerReachedCreditLimit);

        await _commandBus.Send(command);
    }

    public async Task Handle(ProductWasOutOfStock @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.ProductWasOutOfStock);

        await _commandBus.Send(command);
    }

    public async Task Handle(OrderCanceled @event, CancellationToken cancellationToken)
    {
        if (@event.PaymentId.HasValue) // if order was paid but canceled
        {
            var command = RequestCancelPayment.Create(
                PaymentId.Of(@event.PaymentId!.Value), 
                PaymentCancellationReason.OrderCanceled);

            await _commandBus.Send(command);
        }
    }
}