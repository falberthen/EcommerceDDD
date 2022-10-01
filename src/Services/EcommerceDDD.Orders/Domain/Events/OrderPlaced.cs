using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record class OrderPlaced : IDomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid QuoteId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<OrderLineDetails> OrderLines { get; private set; }
    public string CurrencyCode { get; private set; }
    public decimal TotalPrice { get; private set; }

    public static OrderPlaced Create(
        Guid orderId,
        Guid quoteId,
        Guid customerId,
        DateTime createAt, 
        IReadOnlyList<OrderLine> orderLines,
        string currencyCode,
        decimal totalPrice)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (quoteId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(quoteId));
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (orderLines.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(orderLines));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));
        if (totalPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(totalPrice));

        return new OrderPlaced(
            orderId,
            quoteId,
            customerId,
            createAt,
            orderLines.Select(ol => new OrderLineDetails(
                ol.ProductItem.ProductId.Value, 
                ol.ProductItem.ProductName, 
                ol.ProductItem.UnitPrice.Amount, 
                ol.ProductItem.Quantity)).ToList(),
            currencyCode,
            totalPrice);
    }

    public OrderPlaced(
        Guid orderId,
        Guid quoteId,
        Guid customerId,
        DateTime createdAt,
        IReadOnlyList<OrderLineDetails> orderLines,
        string currencyCode,
        decimal totalPrice)
    {
        OrderId = orderId;
        QuoteId = quoteId;
        CustomerId = customerId;
        CreatedAt = createdAt;
        OrderLines = orderLines;
        CurrencyCode = currencyCode;
        TotalPrice = totalPrice;
    }
}

public record OrderLineDetails(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);