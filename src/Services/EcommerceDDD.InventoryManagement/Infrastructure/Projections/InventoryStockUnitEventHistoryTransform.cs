namespace EcommerceDDD.InventoryManagement.Infrastructure.Projections;

public class InventoryStockUnitEventHistoryTransform : EventProjection
{
    public InventoryStockUnitEventHistory Transform(JasperFx.Events.IEvent<UnitEnteredInStock> @event) =>
        InventoryStockUnitEventHistory.Create(@event, @event.Data.InventoryStockUnitId);

    public InventoryStockUnitEventHistory Transform(JasperFx.Events.IEvent<StockQuantityDecreased> @event) =>
        InventoryStockUnitEventHistory.Create(@event, @event.Data.InventoryStockUnitId);

    public InventoryStockUnitEventHistory Transform(JasperFx.Events.IEvent<StockQuantityIncreased> @event) =>
        InventoryStockUnitEventHistory.Create(@event, @event.Data.InventoryStockUnitId);
}