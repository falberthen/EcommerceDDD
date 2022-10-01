using Marten.Events;
using Newtonsoft.Json;
using EcommerceDDD.Core.Infrastructure.Marten;

namespace EcommerceDDD.Quotes.Application.Quotes.GettingQuoteHistory;

public record QuoteEventHistory(
    Guid Id, 
    Guid AggregateId,
    string EventTypeName,
    string EventData) : IEventHistory
{
    public static QuoteEventHistory Create(IEvent @event, Guid aggregateId)
    {
        var serialized = JsonConvert.SerializeObject(@event.Data);
        return new QuoteEventHistory(
            Guid.NewGuid(), 
            aggregateId, 
            @event.EventTypeName, 
            serialized);
    }
}