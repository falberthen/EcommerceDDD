using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record class OrderPlaced(
    OrderId OrderId,
    QuoteId QuoteId,
    CustomerId CustomerId,
    DateTime CreatedAt,
    IReadOnlyList<OrderLine> OrderLines,
    Currency Currency,
    Money TotalPrice) : IDomainEvent
{
    public static OrderPlaced Create(OrderId orderId, 
        QuoteId quoteId, 
        CustomerId customerId,
        DateTime createAt, 
        IReadOnlyList<OrderLine> orderLines,
        Currency currency,
        Money totalPrice)
    {
        return new OrderPlaced(
            orderId,
            quoteId,
            customerId,
            createAt,
            orderLines,
            currency,
            totalPrice);
    }
}