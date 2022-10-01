using Marten.Events;
using Newtonsoft.Json;
using EcommerceDDD.Core.Infrastructure.Marten;

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