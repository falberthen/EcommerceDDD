namespace EcommerceDDD.InventoryManagement.Application.GettingInventoryStockUnitEventHistory;

public record class GetInventoryStockUnitEventHistory : IQuery<IList<InventoryStockUnitEventHistory>>
{
    public ProductId ProductId { get; private set; }

    public static GetInventoryStockUnitEventHistory Create(
        ProductId productId)
    {
        if (productId is null)
            throw new ArgumentNullException(nameof(productId));

        return new GetInventoryStockUnitEventHistory(productId);
    }

    private GetInventoryStockUnitEventHistory(ProductId productId)
    {
        ProductId = productId;
    }
}