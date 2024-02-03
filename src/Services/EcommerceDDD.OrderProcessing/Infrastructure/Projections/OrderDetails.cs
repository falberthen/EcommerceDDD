namespace EcommerceDDD.OrderProcessing.Infrastructure.Projections;

public class OrderDetails
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; } = default;
    public DateTime? ShippedAt { get; set; } = default;
    public DateTime? CanceledAt { get; set; } = default;
    public OrderStatus OrderStatus { get; set; }
    public IList<OrderLineDetails> OrderLines { get; set; } = default!;
    public Currency Currency { get; set; } = default!;
    public decimal TotalPrice { get; set; }

    internal void Apply(OrderPlaced @event)
    {
        Id = @event.OrderId;
        CustomerId = @event.CustomerId;
        QuoteId = @event.QuoteId;
        CreatedAt = @event.Timestamp;
        OrderStatus = OrderStatus.Placed;
    }

    internal void Apply(OrderProcessed @event)
    {
        OrderLines = BuildOrderLines(@event.OrderLines);
        Currency = Currency.OfCode(@event.CurrencyCode);
        TotalPrice = @event.TotalPrice;
    }

    internal void Apply(OrderPaid paid)
    {
        PaymentId = paid.PaymentId;
        OrderStatus = OrderStatus.Paid;
    }

    internal void Apply(OrderShipped @event)
    {
        ShippedAt = @event.Timestamp;
        OrderStatus = OrderStatus.Shipped;
    }

    internal void Apply(OrderCompleted @event)
    {
        CompletedAt = @event.Timestamp;
        OrderStatus = OrderStatus.Completed;
    }

    internal void Apply(OrderCanceled @event)
    {
        CanceledAt = @event.Timestamp;
        OrderStatus = OrderStatus.Canceled;
    }

    private List<OrderLineDetails> BuildOrderLines(IReadOnlyList<OrderLineDetails> orderLines)
    {
        var orderLinesDetails = orderLines.Select(c =>
            new OrderLineDetails(
                c.ProductId,
                c.ProductName,
                c.UnitPrice,
                c.Quantity)
            ).ToList();

        return orderLinesDetails;
    }
}