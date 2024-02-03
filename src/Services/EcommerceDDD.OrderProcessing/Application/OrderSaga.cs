namespace EcommerceDDD.OrderProcessing.Application;

public class OrderSaga :
    IEventHandler<OrderPlaced>,
    IEventHandler<OrderProcessed>,
    IEventHandler<PaymentFinalized>,
    IEventHandler<ShipmentFinalized>
{
    private readonly ICommandBus _commandBus;

    public OrderSaga(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    /// <summary>
    /// Processing placed order
    /// </summary>
    /// <param name="@domainEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(OrderPlaced @domainEvent,
        CancellationToken cancellationToken)
    {
        var processOrderCommand = ProcessOrder.Create(
            CustomerId.Of(@domainEvent.CustomerId),
            OrderId.Of(@domainEvent.OrderId),
            QuoteId.Of(@domainEvent.QuoteId)
        );

        await _commandBus.SendAsync(processOrderCommand, cancellationToken);
    }

    /// <summary>
    /// Requesting payment
    /// </summary>
    /// <param name="@domainEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(OrderProcessed @domainEvent,
        CancellationToken cancellationToken)
    {
        var requestPaymentCommand = RequestPayment.Create(
            CustomerId.Of(@domainEvent.CustomerId),
            OrderId.Of(@domainEvent.OrderId),
            Money.Of(@domainEvent.TotalPrice, @domainEvent.CurrencyCode),
            Currency.OfCode(@domainEvent.CurrencyCode));

        await _commandBus.SendAsync(requestPaymentCommand, cancellationToken);
    }

    /// <summary>
    /// Requesting shipment
    /// </summary>
    /// <param name="@integrationEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(PaymentFinalized @integrationEvent,
        CancellationToken cancellationToken)
    {
        // recording payment        
        var recordPaymentCommand = RecordPayment.Create(
            OrderId.Of(@integrationEvent.OrderId),
            PaymentId.Of(@integrationEvent.PaymentId),
            Money.Of(@integrationEvent.TotalAmount,
                @integrationEvent.CurrencyCode));
        await _commandBus.SendAsync(recordPaymentCommand, cancellationToken);

        // requesting shipment        
        var requestShipmentCommand = RequestShipment.Create(
            OrderId.Of(@integrationEvent.OrderId));
        await _commandBus.SendAsync(requestShipmentCommand, cancellationToken);
    }

    /// <summary>
    /// Completing order
    /// </summary>
    /// <param name="@integrationEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(ShipmentFinalized @integrationEvent,
        CancellationToken cancellationToken)
    {
        // recording shipment
        var recordShipmentCommand = RecordShipment.Create(
            OrderId.Of(@integrationEvent.OrderId),
            ShipmentId.Of(@integrationEvent.ShipmentId));
        await _commandBus.SendAsync(recordShipmentCommand, cancellationToken);

        // completing order
        var completeOrderCommand = CompleteOrder.Create(
            OrderId.Of(@integrationEvent.OrderId),
            ShipmentId.Of(@integrationEvent.ShipmentId));
        await _commandBus.SendAsync(completeOrderCommand, cancellationToken);
    }

}