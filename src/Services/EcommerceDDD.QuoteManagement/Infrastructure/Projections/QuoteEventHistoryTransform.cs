using EcommerceDDD.QuoteManagement.Application.GettingQuoteHistory;

namespace EcommerceDDD.QuoteManagement.Infrastructure.Projections;

public class QuoteEventHistoryTransform : EventProjection
{
    public QuoteEventHistory Transform(JasperFx.Events.IEvent<QuoteOpen> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(JasperFx.Events.IEvent<QuoteItemAdded> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(JasperFx.Events.IEvent<QuoteItemQuantityChanged> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(JasperFx.Events.IEvent<QuoteItemRemoved> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);

    public QuoteEventHistory Transform(JasperFx.Events.IEvent<QuoteCanceled> @event) =>
        QuoteEventHistory.Create(@event, @event.Data.QuoteId);
}