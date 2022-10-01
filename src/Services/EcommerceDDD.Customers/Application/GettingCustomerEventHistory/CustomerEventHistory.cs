using Marten.Events;
using Newtonsoft.Json;
using EcommerceDDD.Core.Infrastructure.Marten;

namespace EcommerceDDD.Customers.Application.GettingCustomerEventHistory;

public record CustomerEventHistory(
    Guid Id,
    Guid AggregateId,
    string EventTypeName,
    string EventData) : IEventHistory
{
    public static CustomerEventHistory Create(IEvent @event, Guid aggregateId)
    {
        var serialized = JsonConvert.SerializeObject(@event.Data);
        return new CustomerEventHistory(
            Guid.NewGuid(),
            aggregateId,
            @event.EventTypeName,
            serialized);
    }
}