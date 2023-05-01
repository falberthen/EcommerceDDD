namespace EcommerceDDD.Quotes.Infrastructure.Projections;

public class QuoteEventHistoryTransform : EventProjection
{
    public QuoteEventHistory Transform(IEvent<QuoteOpen> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(IEvent<QuoteItemAdded> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(IEvent<QuoteItemQuantityChanged> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(IEvent<QuoteItemRemoved> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(IEvent<QuoteCanceled> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);
}