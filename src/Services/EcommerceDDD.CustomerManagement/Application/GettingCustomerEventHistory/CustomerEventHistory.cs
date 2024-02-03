namespace EcommerceDDD.CustomerManagement.Application.GettingCustomerEventHistory;

public record CustomerEventHistory(
    Guid Id,
    Guid AggregateId,
    string EventTypeName,
    string EventData,
    DateTime Timestamp) : IEventHistory
{
    public static CustomerEventHistory Create(IEvent @event, Guid aggregateId)
    {
        var serialized = JsonConvert.SerializeObject(@event.Data);
        return new CustomerEventHistory(
            Guid.NewGuid(),
            aggregateId,
            @event.EventTypeName,
            serialized,
            DateTime.UtcNow);
    }
}