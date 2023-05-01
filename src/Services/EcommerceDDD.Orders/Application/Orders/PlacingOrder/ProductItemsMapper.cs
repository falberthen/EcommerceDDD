using static EcommerceDDD.Orders.Application.Orders.PlacingOrder.PlaceOrderHandler;

namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public class ProductItemsMapper : IProductItemsMapper
{
    private readonly IIntegrationHttpService _integrationHttpService;

    public ProductItemsMapper(IIntegrationHttpService integrationHttpService)
    {
        _integrationHttpService = integrationHttpService;
    }

    /// <summary>
    /// Match the products from Catalog and sets Name and Price
    /// </summary>
    /// <param name="quoteItems"></param>
    /// <param name="currency"></param>
    /// <returns></returns>
    public async Task<List<ProductItemData>> MatchProductsFromCatalog(
        IReadOnlyList<QuoteItemViewModel> quoteItems, Currency currency)
    {
        var productItemsData = new List<ProductItemData>();
        var producIds = quoteItems
            .Select(pid => pid.ProductId)
            .ToArray();

        var products = await _integrationHttpService.FilterAsync<List<ProductViewModel>>(
            "api/products",
            new GetProductsRequest(currency.Code, producIds));

        if (products is null || !products!.Success)
            throw new RecordNotFoundException("An error occurred retrieving products.");

        foreach (var quoteItem in quoteItems)
        {
            var product = products.Data!.FirstOrDefault(p => p.ProductId == quoteItem.ProductId);

            if (product is null)
                throw new RecordNotFoundException($"The product {quoteItem.ProductId} was not found.");

            if (string.IsNullOrEmpty(product.Name))
                throw new ApplicationLogicException($"The product {product.ProductId} has no name.");

            // Setting the more recent product name and price
            productItemsData.Add(new ProductItemData()
            {
                ProductId = ProductId.Of(product.ProductId),
                ProductName = product.Name,
                Quantity = quoteItem.Quantity,
                UnitPrice = Money.Of(product.Price, currency.Code)
            });
        }

        return productItemsData;
    }
}

public record class GetProductsRequest(string CurrencyCode, Guid[] ProductIds);
public record class ProductViewModel(Guid ProductId, string Name, decimal Price, string CurrencySymbol);
