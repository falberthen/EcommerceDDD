namespace EcommerceDDD.Orders.Domain.Events;

public record OrderPaid : IDomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid PaymentId { get; private set; }
    public IList<Guid> OrderLineProducts { get; private set; }
    public string CurrencyCode { get; private set; }
    public decimal TotalPaid { get; private set; }

    public static OrderPaid Create(
        Guid orderId,
        Guid paymentId,
        IList<Guid> orderLineProducts,        
        string currencyCode,
        decimal totalPaid)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (orderLineProducts.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(orderLineProducts));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));
        if (totalPaid < 0)
            throw new ArgumentOutOfRangeException(nameof(totalPaid));

        return new OrderPaid(
            orderId, 
            paymentId, 
            orderLineProducts, 
            currencyCode,
            totalPaid);
    }

    private OrderPaid(
        Guid orderId,
        Guid paymentId,
        IList<Guid> orderLineProducts,
        string currencyCode,
        decimal totalPaid)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        OrderLineProducts = orderLineProducts;
        CurrencyCode = currencyCode;
        TotalPaid = totalPaid;
    }
}
