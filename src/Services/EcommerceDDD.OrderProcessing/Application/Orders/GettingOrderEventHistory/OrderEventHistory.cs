namespace EcommerceDDD.OrderProcessing.Application.GettingOrderEventHistory;

public record OrderEventHistory(
    Guid Id, 
    Guid AggregateId,
    string EventTypeName,
    string EventData,
    DateTime Timestamp) : IEventHistory
{
    public static OrderEventHistory Create(IEvent @event, Guid aggregateId)
    {
        var serialized = JsonConvert.SerializeObject(@event.Data);
        return new OrderEventHistory(
            Guid.NewGuid(), 
            aggregateId, 
            @event.EventTypeName, 
            serialized,
            DateTime.UtcNow);
    }
}