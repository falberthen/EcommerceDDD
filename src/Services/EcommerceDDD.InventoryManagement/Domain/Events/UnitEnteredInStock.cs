namespace EcommerceDDD.InventoryManagement.Domain.Events;

public record class UnitEnteredInStock : DomainEvent
{
    public Guid InventoryStockUnitId { get; private set; }
    public Guid ProductId { get; private set; }
    public int InitialQuantity { get; private set; }

    public static UnitEnteredInStock Create(
        Guid inventoryStockUnitId,
        Guid productId,
        int initialQuantity)
    {
        if (inventoryStockUnitId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inventoryStockUnitId));
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        return new UnitEnteredInStock(
            inventoryStockUnitId,
            productId,
            initialQuantity);
    }

    private UnitEnteredInStock(
        Guid inventoryStockUnitId,
        Guid productId,
        int initialQuantity)
    {
        InventoryStockUnitId = inventoryStockUnitId;
        ProductId = productId;
        InitialQuantity = initialQuantity;
    }
}
