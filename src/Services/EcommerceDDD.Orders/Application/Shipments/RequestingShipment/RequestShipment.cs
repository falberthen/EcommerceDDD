using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

public record class RequestShipment : ICommand
{
    public OrderId OrderId { get; private set; }

    public static RequestShipment Create(OrderId orderId)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));

        return new RequestShipment(orderId);
    }

    private RequestShipment(OrderId orderId)
    {
        OrderId = orderId;
    }
}