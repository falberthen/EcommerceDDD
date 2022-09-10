using EcommerceDDD.IntegrationServices.Products.Responses;

namespace EcommerceDDD.IntegrationServices.Products;

public interface IProductsService
{
    Task<List<ProductViewModel>> GetProductByIds(string apiGatewayUrl, Guid[] productIds, string currencyCode);
    Task<List<ProductInStockViewModel>> CheckProducStockAvailability(string apiGatewayUrl, Guid[] productIds);
}
