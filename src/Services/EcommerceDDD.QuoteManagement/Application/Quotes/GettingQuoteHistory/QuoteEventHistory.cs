namespace EcommerceDDD.QuoteManagement.Application.Quotes.GettingQuoteHistory;

public record QuoteEventHistory(
    Guid Id, 
    Guid AggregateId,
    string EventTypeName,
    string EventData,
    DateTime Timestamp) : IEventHistory
{
    public static QuoteEventHistory Create(IEvent @event, Guid aggregateId)
    {
        var serialized = JsonConvert.SerializeObject(@event.Data);
        return new QuoteEventHistory(
            Guid.NewGuid(), 
            aggregateId, 
            @event.EventTypeName, 
            serialized,
            DateTime.UtcNow);
    }
}