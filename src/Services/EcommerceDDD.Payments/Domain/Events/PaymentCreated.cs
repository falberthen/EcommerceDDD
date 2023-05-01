namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentCreated : IDomainEvent
{
    public Guid PaymentId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PaymentCreated Create(
        Guid paymentId,
        Guid customerId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode,
        DateTime createdAt)
    {        
        if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (totalAmount <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalAmount));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));
        if (createdAt == DateTime.MinValue)
            throw new ArgumentOutOfRangeException(nameof(createdAt));

        return new PaymentCreated(
            paymentId,
            customerId,
            orderId, 
            totalAmount,
            currencyCode,
            createdAt);
    }

    private PaymentCreated(
        Guid paymentId,
        Guid customerId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode,
        DateTime createdAt)
    {
        PaymentId = paymentId;
        CustomerId = customerId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
        CreatedAt = createdAt;
    }
}