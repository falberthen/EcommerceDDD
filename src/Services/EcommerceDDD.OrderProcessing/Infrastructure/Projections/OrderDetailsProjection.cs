namespace EcommerceDDD.OrderProcessing.Infrastructure.Projections;

public partial class OrderDetailsProjection : SingleStreamProjection<OrderDetails, Guid>
{
    public static void Apply(OrderDetails item, OrderPlaced @event) => item.Apply(@event);
    public static void Apply(OrderDetails item, OrderProcessed @event) => item.Apply(@event);
    public static void Apply(OrderDetails item, OrderPaid @event) => item.Apply(@event);
    public static void Apply(OrderDetails item, OrderShipped @event) => item.Apply(@event);
    public static void Apply(OrderDetails item, OrderDelivered @event) => item.Apply(@event);
    public static void Apply(OrderDetails item, OrderCanceled @event) => item.Apply(@event);
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream
