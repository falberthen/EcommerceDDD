using static EcommerceDDD.IntegrationServices.Products.Responses.ProductsResponse;
using static EcommerceDDD.IntegrationServices.Products.Responses.ProductStockAvailabilityResponse;

namespace EcommerceDDD.IntegrationServices.Products;

public interface IProductsService
{
    Task<List<ProductViewModel>> GetProductByIds(string apiGatewayUrl, Guid[] productIds, string currencyCode);
    Task<List<ProductInStockViewModel>> CheckProducStockAvailability(string apiGatewayUrl, Guid[] productIds);
}
