namespace EcommerceDDD.Shipments.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<ShipmentDetailsProjection>(ProjectionLifecycle.Inline);
    }
}
