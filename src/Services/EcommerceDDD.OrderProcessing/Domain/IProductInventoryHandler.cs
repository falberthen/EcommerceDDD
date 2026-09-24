namespace EcommerceDDD.OrderProcessing.Domain;

/// <summary>
/// Infrastructure service for performing operations with inventory.
/// </summary>
public interface IProductInventoryHandler
{
	Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken);
	Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken);
	Task IncreaseQuantityInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken);
}
