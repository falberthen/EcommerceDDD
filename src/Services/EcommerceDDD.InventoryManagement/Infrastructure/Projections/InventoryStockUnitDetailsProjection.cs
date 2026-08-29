namespace EcommerceDDD.InventoryManagement.Infrastructure.Projections;

public partial class InventoryStockUnitDetailsProjection : SingleStreamProjection<InventoryStockUnitDetails, Guid>
{
    public static void Apply(InventoryStockUnitDetails item, UnitEnteredInStock @event) => item.Apply(@event);
    public static void Apply(InventoryStockUnitDetails item, StockQuantityDecreased @event) => item.Apply(@event);
    public static void Apply(InventoryStockUnitDetails item, StockQuantityIncreased @event) => item.Apply(@event);
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream