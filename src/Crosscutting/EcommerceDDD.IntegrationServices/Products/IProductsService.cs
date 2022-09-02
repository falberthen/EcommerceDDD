using static EcommerceDDD.IntegrationServices.Products.ProductsResponse;

namespace EcommerceDDD.IntegrationServices.Products;

public interface IProductsService
{
    Task<List<ProductViewModel>> GetProductByIds(string apiGatewayUrl, Guid[] productIds, string currencyCode);
}
