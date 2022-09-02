using Marten;
using EcommerceDDD.Orders.Application.GettingOrderEventHistory;
using EcommerceDDD.Orders.Application.Orders.GettingOrders;
using Marten.Events.Projections;

namespace EcommerceDDD.Orders.API.Configurations;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<OrderDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<OrderEventHistoryTransformation>();
    }
}
