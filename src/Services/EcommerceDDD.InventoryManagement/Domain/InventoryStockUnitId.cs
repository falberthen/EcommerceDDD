namespace EcommerceDDD.InventoryManagement.Domain;

public sealed class InventoryStockUnitId : StronglyTypedId<Guid>
{
    public static InventoryStockUnitId Of(Guid value)
    {
        return new InventoryStockUnitId(value);
    }

    public InventoryStockUnitId(Guid value) : base(value)
    {
    }
}