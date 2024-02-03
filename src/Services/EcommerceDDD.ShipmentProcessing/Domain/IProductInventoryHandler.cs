namespace EcommerceDDD.ShipmentProcessing.Domain;

/// <summary>
/// Infrastructure service for performing operations with inventory
/// </summary>
public interface IProductInventoryHandler
{
    Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItem> productItems);
    Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItem> productItems);
}
