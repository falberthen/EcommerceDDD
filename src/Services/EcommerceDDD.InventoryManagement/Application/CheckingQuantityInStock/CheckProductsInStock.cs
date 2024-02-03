namespace EcommerceDDD.InventoryManagement.Application.CheckingQuantityInStock;

public record class CheckProductsInStock : IQuery<IList<InventoryStockUnitViewModel>>
{
    public IList<ProductId> ProductIds { get; private set; }

    public static CheckProductsInStock Create(
        IList<ProductId> productIds)
    {
        return new CheckProductsInStock(productIds);
    }

    private CheckProductsInStock(
        IList<ProductId> productIds)
    {
        ProductIds = productIds;
    }
}