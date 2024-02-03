namespace EcommerceDDD.ShipmentProcessing.Infrastructure.InventoryHandling;

public class ProductInventoryHandler : IProductInventoryHandler
{
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly IConfiguration _configuration;

    public ProductInventoryHandler(
        IIntegrationHttpService integrationHttpService,
        IConfiguration configuration)
    {
        _integrationHttpService = integrationHttpService;
        _configuration = configuration;
    }

    public async Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItem> productItems)
    {
        var apiRoute = _configuration["ApiRoutes:InventoryManagement"];
        foreach (var productItem in productItems)
        {
            var response = await _integrationHttpService
                .PutAsync($"{apiRoute}/decrease-stock-quantity",
                    new DecreaseQuantityInStockRequest(
                        productItem.ProductId.Value,
                        productItem.Quantity));

            if (response?.Success == false)
                throw new ApplicationLogicException(
                    $"An error occurred decreasing stock quantity for product {productItem.ProductId.Value}.");
        }
    }

    public async Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItem> productItems)
    {
        var productIdFilter = productItems
            .Select(pid => pid.ProductId.Value).ToArray();

        var apiRoute = _configuration["ApiRoutes:InventoryManagement"];
        var response = await _integrationHttpService
            .FilterAsync<List<ProductInStockViewModel>>(
                $"{apiRoute}/check-stock-quantity",
                new CheckProductsInStockRequest(productIdFilter));

        if (response?.Success == false)
            throw new ApplicationLogicException("An error occurred checking products stock availability.");
        var inventoryResponseData = response.Data
            ?? throw new RecordNotFoundException("No data was provided for the filtered products.");

        foreach (var productItem in productItems)
        {
            var productInStock = inventoryResponseData
                .Single(p => p.ProductId == productItem.ProductId.Value);

            if (productItem.Quantity > productInStock.QuantityInStock)
                return false;
        }

        return true;
    }
}

public record class ProductInStockViewModel(Guid ProductId, int QuantityInStock);
public record class CheckProductsInStockRequest(Guid[] ProductIds);
public record class DecreaseQuantityInStockRequest(Guid ProductId, int DecreasedQuantity);