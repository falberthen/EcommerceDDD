using EcommerceDDD.ServiceClients.ProductCatalog.Models;

namespace EcommerceDDD.ServiceClients.Services.ProductCatalog;

public interface IProductCatalogService
{
    Task<IList<ProductViewModel>?> GetProductsAsync(string currencyCode, IList<Guid?> productIds, CancellationToken cancellationToken);
}
