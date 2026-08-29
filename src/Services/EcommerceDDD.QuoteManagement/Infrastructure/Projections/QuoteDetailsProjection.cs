namespace EcommerceDDD.QuoteManagement.Infrastructure.Projections;

public partial class QuoteDetailsProjection : SingleStreamProjection<QuoteDetails, Guid>
{
    public static void Apply(QuoteDetails item, QuoteOpen @event) => item.Apply(@event);
    public static void Apply(QuoteDetails item, QuoteItemAdded @event) => item.Apply(@event);
    public static void Apply(QuoteDetails item, QuoteItemQuantityChanged @event) => item.Apply(@event);
    public static void Apply(QuoteDetails item, QuoteItemRemoved @event) => item.Apply(@event);
    public static void Apply(QuoteDetails item, QuoteCanceled @event) => item.Apply(@event);
    public static void Apply(QuoteDetails item, QuoteConfirmed @event) => item.Apply(@event);
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream