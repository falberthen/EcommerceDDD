using Marten;
using Marten.Events.Projections;

namespace EcommerceDDD.Orders.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<OrderDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<OrderEventHistoryTransform>(ProjectionLifecycle.Inline);
    }
}
