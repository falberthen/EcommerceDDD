namespace EcommerceDDD.OrderProcessing.Domain;

/// <summary>
/// Infrastructure service for performing operations with inventory.
/// </summary>
public interface IProductInventoryHandler
{
	Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken);	
	Task<bool> DecreaseQuantityInStockAsync(IReadOnlyList<ProductItemData> items, OrderId orderId, CancellationToken cancellationToken);
	Task IncreaseQuantityInStockAsync(IReadOnlyList<ProductItemData> items, OrderId orderId, CancellationToken cancellationToken);
}
