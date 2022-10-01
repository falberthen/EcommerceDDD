namespace EcommerceDDD.Shipments.Domain;

public interface IProductAvailabilityChecker
{
    Task<bool> EnsureProductsInStock(IReadOnlyList<ProductItem> productItems);
}
