namespace EcommerceDDD.InventoryManagement.Infrastructure.Projections;

public static class ProjectionsConfiguration
{
    internal static void ConfigureProjections(this StoreOptions options)
    {
        options.Projections.Add<InventoryStockUnitDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<InventoryStockUnitEventHistoryTransform>(ProjectionLifecycle.Inline);
    }
}