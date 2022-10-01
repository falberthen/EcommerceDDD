using EcommerceDDD.Quotes.Domain.Events;
using Marten.Events.Aggregation;

namespace EcommerceDDD.Quotes.Infrastructure.Projections;

public class QuoteDetailsProjection : SingleStreamAggregation<QuoteDetails>
{
    public QuoteDetailsProjection()
    {
        ProjectEvent<QuoteOpen>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteItemAdded>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteItemQuantityChanged>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteItemRemoved>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteCanceled>((item, @event) => item.Apply(@event));
        ProjectEvent<QuoteConfirmed>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream