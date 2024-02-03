namespace EcommerceDDD.OrderProcessing.Domain;

public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public QuoteId QuoteId { get; private set; }
    public PaymentId? PaymentId { get; private set; }
    public ShipmentId? ShipmentId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public IReadOnlyList<OrderLine> OrderLines => _orderLines;
    public Money TotalPrice { get; private set; }
    public OrderStatus Status { get; private set; }

    private List<OrderLine> _orderLines = default!;

    public static Order Place(OrderData orderData)
    {
        if (orderData.CustomerId is null)
            throw new BusinessRuleException("The customer Id is required.");
        if (orderData.QuoteId is null)
            throw new BusinessRuleException("The quote Id is required.");        

        return new Order(orderData);
    }

    public void Process(OrderData orderData)
    {
        if (orderData?.Items is null || !orderData.Items.Any())
            throw new BusinessRuleException("There's no items to process.");
        if (orderData.Currency is null)
            throw new BusinessRuleException("The currency is required.");

        var orderLines = BuildOrderLines(orderData);
        var totalPrice = CalculateTotalPrice(orderLines, orderData?.Currency!);

        var @event = OrderProcessed.Create(
            orderData!.CustomerId.Value,
            Id.Value,
            orderLines,
            orderData.Currency!.Code,
            totalPrice.Amount);

        AppendEvent(@event);
        Apply(@event);
    }

    public void RecordPayment(PaymentId paymentId, Money totalPaid)
    {
        if (Status != OrderStatus.Processed)
            throw new BusinessRuleException("The order must be processed before paid.");

        var productsIds = OrderLines
            .Select(ol => ol.ProductItem.ProductId.Value).ToList();

        var @event = OrderPaid.Create(
            Id.Value,
            paymentId.Value,
            productsIds,
            totalPaid.Currency.Code,
            totalPaid.Amount);

        AppendEvent(@event);
        Apply(@event);
    }

    public void RecordShipment(ShipmentId shipmentId)
    {
        if (Status != OrderStatus.Paid)
            throw new BusinessRuleException("The order must be paid before shipped.");

        var productsIds = OrderLines
            .Select(ol => ol.ProductItem.ProductId.Value).ToList();
        var @event = OrderShipped.Create(
            Id.Value,
            shipmentId.Value,
            productsIds);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Complete(ShipmentId shipmentId)
    {
        if (Status != OrderStatus.Shipped)
            throw new BusinessRuleException("The order must be shipped before completed.");

        var @event = OrderCompleted.Create(
            Id.Value,
            shipmentId.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel(OrderCancellationReason cancellationReason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Canceled)
            throw new BusinessRuleException("The order cannot be cancelled at this point.");

        var @event = OrderCanceled.Create(
            Id.Value,
            PaymentId?.Value,
            cancellationReason);

        AppendEvent(@event);
        Apply(@event);
    }

    private List<OrderLine> BuildOrderLines(OrderData orderData)
    {
        var orderLines = orderData.Items.Select(c =>
            OrderLine.Create(
                new ProductItem(
                    c.ProductId,
                    c.ProductName,
                    c.UnitPrice,
                    c.Quantity,
                    orderData.Currency)
                )
            ).ToList();

        return orderLines;
    }

    private void Apply(OrderPlaced @event)
    {
        Id = OrderId.Of(@event.OrderId);
        CustomerId = CustomerId.Of(@event.CustomerId);
        QuoteId = QuoteId.Of(@event.QuoteId);
        CreatedAt = @event.Timestamp;
        Status = OrderStatus.Placed;
    }

    private void Apply(OrderProcessed @event)
    {
        TotalPrice = Money.Of(@event.TotalPrice, @event.CurrencyCode);

        var currency = Currency.OfCode(@event.CurrencyCode);
        _orderLines = @event.OrderLines.Select(ol =>
            OrderLine.Create(
                new ProductItem(
                    ProductId.Of(ol.ProductId),
                    ol.ProductName,
                    Money.Of(ol.UnitPrice, currency.Code),
                    ol.Quantity,
                    currency)))
            .ToList();

        Status = OrderStatus.Processed;
    }

    private void Apply(OrderPaid @event)
    {
        PaymentId = PaymentId.Of(@event.PaymentId);
        Status = OrderStatus.Paid;
    }

    private void Apply(OrderCompleted @event)
    {
        Status = OrderStatus.Completed;
        ShipmentId = ShipmentId.Of(@event.ShipmentId);
        CompletedAt = @event.Timestamp;
    }

    private void Apply(OrderCanceled @event)
    {
        Status = OrderStatus.Canceled;
        CanceledAt = @event.Timestamp;
    }

    private void Apply(OrderShipped @event)
    {
        Status = OrderStatus.Shipped;
        ShippedAt = @event.Timestamp;
    }

    private Money CalculateTotalPrice(List<OrderLine> orderLines, Currency currency)
    {
        var amount = orderLines.Sum(qi =>
        {
            return (qi.ProductItem.Quantity * qi.ProductItem.UnitPrice).Amount;
        });

        return Money.Of(amount, currency.Code);
    }

    private Order(OrderData orderData)
    {
        Guid orderId = Guid.NewGuid();
        var @event = OrderPlaced.Create(
            orderData.CustomerId.Value,
            orderId,
            orderData.QuoteId.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    private Order() { }
}
