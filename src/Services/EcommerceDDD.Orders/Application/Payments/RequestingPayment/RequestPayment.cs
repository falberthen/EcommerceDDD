using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Payments.RequestingPayment;

public record class RequestPayment: ICommand
{
    public CustomerId CustomerId { get; set; }
    public OrderId OrderId { get; set; }
    public Money TotalPrice { get; set; }
    public Currency Currency { get; set; }

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
        Money totalPrice, 
        Currency currency)
    {
        CustomerId = customerId;
        OrderId = orderId;
        TotalPrice = totalPrice;
        Currency = currency;
    }

}