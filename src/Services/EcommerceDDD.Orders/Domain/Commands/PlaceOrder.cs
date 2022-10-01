using EcommerceDDD.Core.CQRS.CommandHandling;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EcommerceDDD.Orders.Domain.Commands;

public record class PlaceOrder : ICommand 
{
    public OrderData OrderData { get; private set; }

    public static PlaceOrder Create(OrderData orderData)
    {
        var (OrderId, QuoteId, CustomerId, Items, Currency) = orderData;

        if (OrderId is null)
            throw new ArgumentNullException(nameof(OrderId));
        if (QuoteId is null)
            throw new ArgumentNullException(nameof(QuoteId));
        if (CustomerId is null)
            throw new ArgumentNullException(nameof(CustomerId));
        if (Items.Count <= 0)
            throw new ArgumentOutOfRangeException(nameof(Items));
        if (Currency is null)
            throw new ArgumentNullException(nameof(Currency));

        return new PlaceOrder(orderData);
    }

    private PlaceOrder(OrderData orderData)
    {
        OrderData = orderData;
    }
}