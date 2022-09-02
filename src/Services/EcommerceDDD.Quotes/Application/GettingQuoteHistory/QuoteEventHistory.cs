using Marten.Events;
using Newtonsoft.Json;
using Marten.Events.Projections;
using EcommerceDDD.Quotes.Domain.Events;
using EcommerceDDD.Core.Infrastructure.Marten;

namespace EcommerceDDD.Quotes.Application.GettingQuoteHistory;

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

// Projection
public class QuoteEventHistoryTransformation : EventProjection
{
    public QuoteEventHistory Transform(IEvent<QuoteCreated> @event) => 
        QuoteEventHistory.Create(@event, @event.Data.QuoteId.Value);

    public QuoteEventHistory Transform(IEvent<QuoteItemAdded> @event) => 
        QuoteEventHistory.Create(@event, @event.Data.QuoteId.Value);

    public QuoteEventHistory Transform(IEvent<QuoteItemQuantityChanged> @event) => 
        QuoteEventHistory.Create(@event, @event.Data.QuoteId.Value);

    public QuoteEventHistory Transform(IEvent<QuoteItemRemoved> @event) => 
        QuoteEventHistory.Create(@event, @event.Data.QuoteId.Value);

    public QuoteEventHistory Transform(IEvent<QuoteCancelled> @event) => 
        QuoteEventHistory.Create(@event, @event.Data.QuoteId.Value);
}