using Marten;
using EcommerceDDD.Quotes.Application.GettingOpenQuote;
using EcommerceDDD.Quotes.Application.GettingQuoteHistory;
using Marten.Events.Projections;

namespace EcommerceDDD.Quotes.API.Configurations;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<QuoteDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<QuoteEventHistoryTransformation>();
    }
}
