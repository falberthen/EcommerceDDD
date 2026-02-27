namespace EcommerceDDD.OrderProcessing.Infrastructure.Projections;

public class OrderEventHistoryTransform : EventProjection
{
    public OrderEventHistory Transform(JasperFx.Events.IEvent<OrderPlaced> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(JasperFx.Events.IEvent<OrderProcessed> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(JasperFx.Events.IEvent<OrderPaid> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(JasperFx.Events.IEvent<OrderShipped> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(JasperFx.Events.IEvent<OrderDelivered> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(JasperFx.Events.IEvent<OrderCanceled> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);
}