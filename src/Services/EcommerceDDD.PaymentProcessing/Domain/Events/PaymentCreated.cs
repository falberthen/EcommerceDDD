namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCreated : DomainEvent
{
    public Guid PaymentId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; }

    public static PaymentCreated Create(
        Guid paymentId,
        Guid customerId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode)
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
 
        return new PaymentCreated(
            paymentId,
            customerId,
            orderId, 
            totalAmount,
            currencyCode);
    }

    private PaymentCreated(
        Guid paymentId,
        Guid customerId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode)
    {
        PaymentId = paymentId;
        CustomerId = customerId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
    }
}