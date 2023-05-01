namespace EcommerceDDD.Customers.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<CustomerDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<CustomerHistoryTransform>(ProjectionLifecycle.Inline);
    }
}