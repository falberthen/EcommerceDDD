using EcommerceDDD.Core.Exceptions;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Core.Infrastructure.Integration;

namespace EcommerceDDD.Shipments.Application.ShippingPackage;

public class ProductAvailabilityChecker : IProductAvailabilityChecker
{
    private readonly IIntegrationHttpService _integrationHttpService;

    public ProductAvailabilityChecker(IIntegrationHttpService integrationHttpService)
    {
        _integrationHttpService = integrationHttpService;
    }

    public async Task<bool> EnsureProductsInStock(IReadOnlyList<ProductItem> productItems)
    {
        var productIdFilter = productItems.Select(pid => pid.ProductId.Value).ToArray();
        var response = await _integrationHttpService
            .FilterAsync<List<ProductInStockViewModel>>(
                "api/products/stockavailability",
                new ProductStockAvailabilityRequest(productIdFilter));

        if (response is null || !response!.Success)
            throw new ApplicationLogicException("An error occurred checking products stock availability.");

        var productsStockAvailability = response.Data
            ?? throw new RecordNotFoundException("No data was provided for the filtered products.");

        foreach (var productItem in productItems)
        {
            var productInStock = productsStockAvailability
                .Single(p => p.ProductId == productItem.ProductId.Value);

            if (productItem.Quantity > productInStock.AmountInStock)
                return false;
        }

        return true;
    }
}
