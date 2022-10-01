using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;

namespace EcommerceDDD.Orders.Infrastructure.Projections;

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

    public void Apply(OrderPlaced placed)
    {
        Id = placed.OrderId;
        QuoteId = placed.QuoteId;
        CustomerId = placed.CustomerId;
        CreatedAt = placed.CreatedAt;
        OrderStatus = OrderStatus.Placed;
        OrderLines = BuildOrderLines(placed.OrderLines);
        Currency = Currency.OfCode(placed.CurrencyCode);
        TotalPrice = placed.TotalPrice;
    }

    public void Apply(OrderPaid paid)
    {
        PaymentId = paid.PaymentId;
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