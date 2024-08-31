namespace EcommerceDDD.InventoryManagement.Domain;

public sealed class InventoryStockUnitId(Guid value) : StronglyTypedId<Guid>(value)
{
    public static InventoryStockUnitId Of(Guid value) => new InventoryStockUnitId(value);
}