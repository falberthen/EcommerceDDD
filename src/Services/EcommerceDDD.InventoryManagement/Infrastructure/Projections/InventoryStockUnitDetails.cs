namespace EcommerceDDD.InventoryManagement.Infrastructure.Projections;

public class InventoryStockUnitDetails
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int QuantityInStock { get; set; }

    internal void Apply(UnitEnteredInStock @event)
    {
        Id = @event.InventoryStockUnitId;
        ProductId = @event.ProductId;
        QuantityInStock = @event.InitialQuantity;
    }

    internal void Apply(StockQuantityDecreased @event)
    {
        ProductId = @event.ProductId;
        QuantityInStock -= @event.QuantityDecreased;
    }

    internal void Apply(StockQuantityIncreased @event)
    {
        ProductId = @event.ProductId;
        QuantityInStock += @event.QuantityIncreased;
    }
}