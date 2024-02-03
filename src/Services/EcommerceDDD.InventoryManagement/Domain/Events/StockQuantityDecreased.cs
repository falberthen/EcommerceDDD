namespace EcommerceDDD.InventoryManagement.Domain.Events;

public record class StockQuantityDecreased : DomainEvent
{
    public Guid InventoryStockUnitId { get; private set; }
    public Guid ProductId { get; private set; }
    public int QuantityDecreased { get; private set; }

    public static StockQuantityDecreased Create(
        Guid inventoryStockUnitId,
        Guid productId,
        int quantityDecreased)
    {
        if (inventoryStockUnitId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inventoryStockUnitId));
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantityDecreased <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantityDecreased));

        return new StockQuantityDecreased(
            inventoryStockUnitId,
            productId,
            quantityDecreased);
    }

    private StockQuantityDecreased(
        Guid inventoryStockUnitId,
        Guid productId,
        int quantityDecreased)
    {
        InventoryStockUnitId = inventoryStockUnitId;
        ProductId = productId;
        QuantityDecreased = quantityDecreased;
    }
}
