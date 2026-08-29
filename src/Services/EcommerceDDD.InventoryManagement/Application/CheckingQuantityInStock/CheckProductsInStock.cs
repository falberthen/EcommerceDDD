namespace EcommerceDDD.InventoryManagement.Application.CheckingQuantityInStock;

public record class CheckProductsInStock : IQuery<IReadOnlyList<InventoryStockUnitViewModel>>
{
	public IReadOnlyList<ProductId> ProductIds { get; private set; }

	public static CheckProductsInStock Create(
		IReadOnlyList<ProductId> productIds) => new CheckProductsInStock(productIds);

	private CheckProductsInStock(
		IReadOnlyList<ProductId> productIds) => ProductIds = productIds;
}