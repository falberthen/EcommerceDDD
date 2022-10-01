using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Payments.Domain.Events;

public record PaymentRequested : IDomainEvent
{
    public Guid PaymentId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; }

    public static PaymentRequested Create(
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

        return new PaymentRequested(
            paymentId,
            customerId,
            orderId, 
            totalAmount,
            currencyCode);
    }

    private PaymentRequested(
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