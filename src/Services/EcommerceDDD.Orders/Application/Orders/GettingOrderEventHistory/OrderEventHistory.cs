using Marten.Events;
using Newtonsoft.Json;
using Marten.Events.Projections;
using EcommerceDDD.Core.Infrastructure.Marten;
using EcommerceDDD.Orders.Domain.Events;

namespace EcommerceDDD.Orders.Application.GettingOrderEventHistory;

public record OrderEventHistory(
    Guid Id, 
    Guid AggregateId,
    string EventTypeName,
    string EventData) : IEventHistory
    {
        public static OrderEventHistory Create(IEvent @event, Guid aggregateId)
        {
            var serialized = JsonConvert.SerializeObject(@event.Data);
            return new OrderEventHistory(
                Guid.NewGuid(), 
                aggregateId, 
                @event.EventTypeName, 
                serialized);
        }
    }

// Projection
public class OrderEventHistoryTransformation : EventProjection
{
    public OrderEventHistory Transform(IEvent<OrderPlaced> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId.Value);

    public OrderEventHistory Transform(IEvent<OrderPaid> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId.Value);

    public OrderEventHistory Transform(IEvent<OrderShipped> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId.Value);

    public OrderEventHistory Transform(IEvent<OrderCompleted> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId.Value);

    public OrderEventHistory Transform(IEvent<OrderCanceled> @event) =>
        OrderEventHistory.Create(@event, @event.Data.OrderId.Value);
}