using Marten;
using Marten.Events.Projections;
using EcommerceDDD.Quotes.Infrastructure.Projections;

namespace EcommerceDDD.Quotes.API.Configurations;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<QuoteDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<QuoteEventHistoryTransform>(ProjectionLifecycle.Inline);
    }
}
