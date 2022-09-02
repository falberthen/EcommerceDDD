using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.Core.Infrastructure.Http;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.IntegrationServices.Products.Requests;
using Microsoft.Extensions.Options;
using static EcommerceDDD.IntegrationServices.Products.ProductsResponse;

namespace EcommerceDDD.IntegrationServices.Products;

public class ProductsService : IProductsService
{
    private readonly IHttpRequester _httpRequester;
    private readonly ITokenRequester _tokenRequester;
    private readonly TokenIssuerSettings _tokenIssuerSettings;

    public ProductsService(
        IHttpRequester httpRequester,
        ITokenRequester tokenRequester,
        IOptions<TokenIssuerSettings> tokenIssuerSettings)
    {
        _httpRequester = httpRequester;
        _tokenRequester = tokenRequester;
        _tokenIssuerSettings = tokenIssuerSettings.Value;
    }

    public async Task<List<ProductViewModel>> GetProductByIds(string apiGatewayUrl, Guid[] productIds, string currencyCode)
    {
        var tokenResponse = await _tokenRequester
            .GetApplicationToken(_tokenIssuerSettings);

        var response = await _httpRequester.PostAsync<ProductsResponse>(
            $"{apiGatewayUrl}/api/products",
            new GetProductsRequest(currencyCode, productIds),
            tokenResponse.AccessToken); ;

        if (!response.Success)
            throw new Exception("Error retrieving products");

        return response.Data;
    }
}