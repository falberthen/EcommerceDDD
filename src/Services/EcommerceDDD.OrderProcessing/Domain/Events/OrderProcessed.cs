namespace EcommerceDDD.OrderProcessing.Domain.Events;

public record class OrderProcessed : DomainEvent
{
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public IReadOnlyList<OrderLineDetails> OrderLines { get; private set; }
    public string CurrencyCode { get; private set; }
    public decimal TotalPrice { get; private set; }

    public static OrderProcessed Create(
        Guid customerId,
        Guid orderId,
        IReadOnlyList<OrderLine> orderLines,
        string currencyCode,
        decimal totalPrice)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));       
        if (orderLines.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(orderLines));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));
        if (totalPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(totalPrice));
  
        return new OrderProcessed(
            customerId,
            orderId,            
            orderLines.Select(ol => new OrderLineDetails(
                ol.ProductItem.ProductId.Value, 
                ol.ProductItem.ProductName, 
                ol.ProductItem.UnitPrice.Amount, 
                ol.ProductItem.Quantity)).ToList(),
            currencyCode,
            totalPrice);
    }

    public OrderProcessed(
        Guid customerId,
        Guid orderId,        
        IReadOnlyList<OrderLineDetails> orderLines,
        string currencyCode,
        decimal totalPrice)
    {
        CustomerId = customerId;
        OrderId = orderId;       
        OrderLines = orderLines;
        CurrencyCode = currencyCode;
        TotalPrice = totalPrice;
    }
}

public record OrderLineDetails(
    Guid ProductId, 
    string ProductName, 
    decimal UnitPrice, 
    int Quantity);