namespace EcommerceDDD.QuoteManagement.Infrastructure.ProductMapping;

public class ProductMapper : IProductMapper
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IConfiguration _configuration;

    public ProductMapper(
        IIntegrationHttpService integrationHttpService,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _configuration = configuration;
    }

    /// <summary>
    /// Maps product from catalog
    /// </summary>
    /// <param name="productIds"></param>
    /// <param name="currency"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    public async Task<IEnumerable<ProductViewModel>> MapProductFromCatalogAsync(
        IEnumerable<ProductId> productIds, Currency currency)
    {
        var productIdValues = productIds.Select(p => p.Value).ToArray();
        var apiRoute = _configuration["ApiRoutes:ProductCatalog"];
        var response = await _integrationHttpService.FilterAsync<List<ProductViewModel>>(
            apiRoute, new GetProductsRequest(currency.Code, productIdValues));

        if (response?.Success == false)
            throw new RecordNotFoundException("An error occurred retrieving products.");

        var productViewModel = response?.Data!;
        return productViewModel;
    }
}

public record class GetProductsRequest(string CurrencyCode, Guid[] ProductIds);