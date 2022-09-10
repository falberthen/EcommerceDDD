using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using Marten.Events.Aggregation;

namespace EcommerceDDD.Orders.Application.Orders.GettingOrders;

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
    public Currency Currency { get; private set; } = default!;
    public decimal TotalPrice { get; set; }

    public void Apply(OrderPlaced placed)
    {
        Id = placed.OrderId.Value;
        QuoteId = placed.QuoteId.Value;
        CustomerId = placed.CustomerId.Value;
        CreatedAt = placed.CreatedAt;
        OrderStatus = OrderStatus.Placed;
        OrderLines = BuildOrderLines(placed.OrderLines);
        Currency = placed.Currency;
        TotalPrice = placed.TotalPrice.Value;
    }

    public void Apply(OrderPaid paid)
    {
        PaymentId = paid.PaymentId.Value;
        OrderStatus = OrderStatus.Paid;
    }

    public void Apply(OrderShipped shipped)
    {
        ShippedAt = shipped.ShippedAt;
        OrderStatus = OrderStatus.Shipped;
    }

    public void Apply(OrderCompleted completed)
    {
        OrderStatus = OrderStatus.Completed;
        CompletedAt = completed.CompletedAt;
    }

    public void Apply(OrderCanceled canceled)
    {
        OrderStatus = OrderStatus.Canceled;
        CanceledAt = canceled.CanceledAt;
    }

    private List<OrderLineDetails> BuildOrderLines(IReadOnlyList<OrderLine> orderLines)
    {
        var orderLinesDetails = orderLines.Select(c =>
            new OrderLineDetails(
                c.ProductItem.ProductId.Value,
                c.ProductItem.ProductName,
                c.ProductItem.UnitPrice.Value,                
                c.ProductItem.Quantity)
            ).ToList();

        return orderLinesDetails;
    }

    public record OrderLineDetails(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);
}

public class OrderDetailsProjection : SingleStreamAggregation<OrderDetails>
{
    public OrderDetailsProjection()
    {
        ProjectEvent<OrderPlaced>((item, @event) => item.Apply(@event));
        ProjectEvent<OrderPaid>((item, @event) => item.Apply(@event));
        ProjectEvent<OrderShipped>((item, @event) => item.Apply(@event));
        ProjectEvent<OrderCanceled>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream