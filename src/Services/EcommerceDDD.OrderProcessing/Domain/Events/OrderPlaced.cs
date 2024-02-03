namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record class OrderPlaced : DomainEvent
{
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid QuoteId { get; private set; }
    
    public static OrderPlaced Create(
        Guid customerId,
        Guid orderId,
        Guid quoteId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));        
        
        return new OrderPlaced(
            customerId,
            orderId,
            quoteId);
    }

    public OrderPlaced(Guid customerId, Guid orderId, Guid quoteId)
    {
        CustomerId = customerId;
        OrderId = orderId;
        QuoteId = quoteId;        
    }
}