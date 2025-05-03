namespace EcommerceDDD.PaymentProcessing.Domain;

/// <summary>
/// Infrastructure service for performing operations with inventory
/// </summary>
public interface IProductInventoryHandler
{
    Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken);	
	Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken);
}