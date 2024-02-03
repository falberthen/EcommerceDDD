namespace EcommerceDDD.InventoryManagement.Domain.Events;

public record class StockQuantityIncreased : DomainEvent
{
    public Guid InventoryStockUnitId { get; private set; }
    public Guid ProductId { get; private set; }
    public int QuantityIncreased { get; private set; }

    public static StockQuantityIncreased Create(
        Guid inventoryStockUnitId,
        Guid productId,
        int quantityIncreased)
    {
        if (inventoryStockUnitId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inventoryStockUnitId));
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));
        if (quantityIncreased <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantityIncreased));

        return new StockQuantityIncreased(
            inventoryStockUnitId,
            productId,
            quantityIncreased);
    }

    private StockQuantityIncreased(
        Guid inventoryStockUnitId,
        Guid productId,
        int quantityIncreased)
    {
        InventoryStockUnitId = inventoryStockUnitId;
        ProductId = productId;
        QuantityIncreased = quantityIncreased;
    }
}
