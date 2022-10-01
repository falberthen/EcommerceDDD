using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Orders.Domain.Events;

namespace EcommerceDDD.Orders.Domain;

public class Order : AggregateRoot<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public QuoteId QuoteId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }
    public PaymentId? PaymentId { get; private set; }
    public ShipmentId? ShipmentId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public IReadOnlyList<OrderLine> OrderLines => _orderLines;

    private List<OrderLine> _orderLines { get; set; } = default!;

    public static Order Create(OrderData orderData)
    {
        var (OrderId, QuoteId, CustomerId, Items, Currency) = orderData
            ?? throw new ArgumentNullException(nameof(orderData));
        
        if (OrderId is null)
            throw new BusinessRuleException("The order Id is required.");

        if (QuoteId is null)
            throw new BusinessRuleException("The quote Id is required.");

        if (CustomerId is null)
            throw new BusinessRuleException("The customer Id is required.");

        if (!Items.Any())
            throw new BusinessRuleException("An order should have at least one product.");

        if (Currency is null)
            throw new BusinessRuleException("The currency is required.");

        return new Order(orderData);
    }

    public void RecordPayment(PaymentId paymentId, Money totalPaid)
    {
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

    public void RecordShippedEvent()
    {
        if (Status != OrderStatus.Paid)
            throw new BusinessRuleException("The order must be paid before shipped.");

        var @event = OrderShipped.Create(
            Id.Value,
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Complete(ShipmentId shipmentId)
    {
        if (Status != OrderStatus.Shipped)
            throw new BusinessRuleException("The order must be shipped before completed.");

        var @event = OrderCompleted.Create(
            Id.Value, 
            shipmentId.Value,
            DateTime.UtcNow);

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
            cancellationReason, 
            DateTime.UtcNow);

        AppendEvent(@event);
        Apply(@event);
    }

    private List<OrderLine> BuildOrderLines(IReadOnlyList<ProductItemData> items)
    {
        var orderLines = items.Select(c =>
            OrderLine.Create(
                new ProductItem(
                    c.ProductId, 
                    c.ProductName, 
                    c.UnitPrice, 
                    c.Quantity)
                )
            ).ToList();

        return orderLines;
    }

    private void Apply(OrderPlaced placed)
    {
        Id = OrderId.Of(placed.OrderId);
        QuoteId = QuoteId.Of(placed.QuoteId);
        CustomerId = CustomerId.Of(placed.CustomerId);
        CreatedAt = placed.CreatedAt;
        Status = OrderStatus.Placed;
        TotalPrice = Money.Of(placed.TotalPrice, placed.CurrencyCode);

        _orderLines = placed.OrderLines.Select(ol =>
            OrderLine.Create(
                new ProductItem(
                ProductId.Of(ol.ProductId),
                ol.ProductName,
                Money.Of(ol.UnitPrice, placed.CurrencyCode),
                ol.Quantity))
            ).ToList();
    }

    private void Apply(OrderPaid paid)
    {
        PaymentId = PaymentId.Of(paid.PaymentId);
        Status = OrderStatus.Paid;
    }

    private void Apply(OrderCompleted completed)
    {
        Status = OrderStatus.Completed;
        ShipmentId = ShipmentId.Of(completed.ShipmentId);
        CompletedAt = completed.CompletedAt;
    }

    private void Apply(OrderCanceled canceled)
    {
        Status = OrderStatus.Canceled;
        CanceledAt = canceled.CanceledAt;
    }

    private void Apply(OrderShipped shipped)
    {
        Status = OrderStatus.Shipped;
        ShippedAt = shipped.ShippedAt;
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
        var orderLines = BuildOrderLines(orderData.Items);
        var totalPrice = CalculateTotalPrice(orderLines, orderData.Currency);

        var @event = OrderPlaced.Create(
            orderData.OrderId.Value,
            orderData.QuoteId.Value,
            orderData.CustomerId.Value,
            DateTime.UtcNow,
            orderLines,
            orderData.Currency.Code,
            totalPrice.Amount);

        AppendEvent(@event);
        Apply(@event);
    }

    private Order() {}
}
