namespace EcommerceDDD.Orders.Infrastructure.Projections;

public class OrderDetailsProjection : SingleStreamProjection<OrderDetails>
{
    public OrderDetailsProjection()
    {
        ProjectEvent<OrderPlaced>((item, @event) => item.Apply(@event));
        ProjectEvent<OrderPaid>((item, @event) => item.Apply(@event));
        ProjectEvent<OrderShipped>((item, @event) => item.Apply(@event));
        ProjectEvent<OrderCanceled>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream