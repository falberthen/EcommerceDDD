namespace EcommerceDDD.InventoryManagement.Infrastructure.Projections;

public class InventoryStockUnitEventHistoryTransform : EventProjection
{
    public InventoryStockUnitEventHistory Transform(IEvent<UnitEnteredInStock> @event) =>
        InventoryStockUnitEventHistory.Create(@event, @event.Data.InventoryStockUnitId);

    public InventoryStockUnitEventHistory Transform(IEvent<StockQuantityDecreased> @event) =>
        InventoryStockUnitEventHistory.Create(@event, @event.Data.InventoryStockUnitId);

    public InventoryStockUnitEventHistory Transform(IEvent<StockQuantityIncreased> @event) =>
        InventoryStockUnitEventHistory.Create(@event, @event.Data.InventoryStockUnitId);
}