namespace EcommerceDDD.InventoryManagement.Infrastructure.Projections;

public class InventoryStockUnitDetailsProjection : SingleStreamProjection<InventoryStockUnitDetails, Guid>
{
    public InventoryStockUnitDetailsProjection()
    {
        ProjectEvent<UnitEnteredInStock>((item, @event) => item.Apply(@event));
        ProjectEvent<StockQuantityDecreased>((item, @event) => item.Apply(@event));
        ProjectEvent<StockQuantityIncreased>((item, @event) => item.Apply(@event));
    }
}

//https://martendb.io/events/projections/aggregate-projections.html#aggregate-by-stream