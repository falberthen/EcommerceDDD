namespace EcommerceDDD.OrderProcessing.Infrastructure.Projections;

public class OrderEventHistoryTransform : EventProjection
{
    public OrderEventHistory Transform(IEvent<OrderPlaced> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(IEvent<OrderProcessed> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(IEvent<OrderPaid> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(IEvent<OrderShipped> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(IEvent<OrderCompleted> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);

    public OrderEventHistory Transform(IEvent<OrderCanceled> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId);
}