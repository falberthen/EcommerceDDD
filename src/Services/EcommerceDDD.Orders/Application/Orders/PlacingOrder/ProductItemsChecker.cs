using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Core.Infrastructure.Integration;

namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public class ProductItemsChecker : IProductItemsChecker
{
    private readonly IIntegrationHttpService _integrationHttpService;

    public ProductItemsChecker(IIntegrationHttpService integrationHttpService)
    {
        _integrationHttpService = integrationHttpService;
    }

    public async Task EnsureProductItemsExist(IReadOnlyList<ProductItemData> productItems, Currency currency)
    {
        var producIds = productItems
            .Select(pid => pid.ProductId.Value)
            .ToArray();

        var products = await _integrationHttpService.FilterAsync<List<ProductViewModel>>(
            "api/products",
            new GetProductsRequest(currency.Code, producIds));

        if (products is null || !products!.Success)
            throw new RecordNotFoundException("An error occurred retrieving products.");

        foreach (var item in productItems)
        {
            var product = products.Data!.FirstOrDefault(p => p.ProductId == item.ProductId.Value);
            if (product is null)
                throw new RecordNotFoundException($"The product {item.ProductId.Value} was not found.");

            if (string.IsNullOrEmpty(product.Name))
                throw new ApplicationLogicException($"The product {product.ProductId} has no name.");

            // Sets the more recent product name and price
            item.SetName(product.Name);
            item.SetPrice(Money.Of(product.Price, currency.Code));
        }
    }
}

public record class GetProductsRequest(string CurrencyCode, Guid[] ProductIds);
public record class ProductViewModel(Guid ProductId, string Name, decimal Price, string CurrencySymbol);
