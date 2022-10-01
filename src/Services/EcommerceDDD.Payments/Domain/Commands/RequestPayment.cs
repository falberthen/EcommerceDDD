using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Payments.Domain.Commands;

public record class RequestPayment : ICommand
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
    public Money TotalAmount { get; private set; }
    public Currency Currency { get; private set; }

    public static RequestPayment Create(
        CustomerId customerId,
        OrderId orderId,
        Money totalPrice,
        Currency currency)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (totalPrice is null)
            throw new ArgumentNullException(nameof(totalPrice));
        if (currency is null)
            throw new ArgumentNullException(nameof(currency));

        return new RequestPayment(customerId, orderId, totalPrice, currency);
    }

    private RequestPayment(
        CustomerId customerId,
        OrderId orderId,
        Money totalAmount,
        Currency currency)
    {
        CustomerId = customerId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        Currency = currency;
    }
}