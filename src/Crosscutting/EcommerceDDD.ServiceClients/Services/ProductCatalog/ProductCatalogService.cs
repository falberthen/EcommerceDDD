using EcommerceDDD.ServiceClients.ProductCatalog;
using EcommerceDDD.ServiceClients.ProductCatalog.Models;

namespace EcommerceDDD.ServiceClients.Services.ProductCatalog;

public class ProductCatalogService(ProductCatalogClient productCatalogClient) : IProductCatalogService
{
    private readonly ProductCatalogClient _productCatalogClient = productCatalogClient;

    public async Task<IList<ProductViewModel>?> GetProductsAsync(string currencyCode, IList<Guid?> productIds, CancellationToken cancellationToken)
    {
        var request = new GetProductsRequest()
        {
            CurrencyCode = currencyCode,
            ProductIds = productIds.ToList()
        };

        return await _productCatalogClient.Api.V2.Products
            .PostAsync(request, cancellationToken: cancellationToken);
    }
}
