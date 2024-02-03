namespace EcommerceDDD.ProductCatalog.Application.Products.GettingProducts;

public class GetProductsHandler : IQueryHandler<GetProducts, IList<ProductViewModel>> 
{
    private readonly IProducts _products;
    private readonly IIntegrationHttpService _integrationHttpService;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly IConfiguration _configuration;

    public GetProductsHandler(
        IProducts products,
        IIntegrationHttpService integrationHttpService,
        ICurrencyConverter currencyConverter,
        IConfiguration configuration)
    {
        _products = products;
        _integrationHttpService = integrationHttpService;
        _currencyConverter = currencyConverter;
        _configuration = configuration;
    }

    public async Task<IList<ProductViewModel>> Handle(GetProducts query, 
        CancellationToken cancellationToken)
    {
        var productsViewModel = new List<ProductViewModel>();
        var products = query.ProductIds.Count == 0
            ? await _products.ListAll(cancellationToken)
            : await _products.GetByIds(query.ProductIds);

        if (string.IsNullOrEmpty(query.CurrencyCode))
            throw new RecordNotFoundException("Currency code cannot be empty.");

        // Getting stock quantity
        var productIds = products.Select(x => x.Id.Value).ToArray();
        var apiRoute = _configuration["ApiRoutes:InventoryManagement"];
        var inventoryResponse = await _integrationHttpService
            .FilterAsync<IList<InventoryStockUnitViewModel>>(
                $"{apiRoute}/check-stock-quantity", 
                new CheckProductsInStockRequest(productIds));

        var currency = Currency.OfCode(query.CurrencyCode);

        foreach (var product in products)
        {
            int quantityInStock = 0;
            if (inventoryResponse?.Success == true)
            {
                var productInStock = inventoryResponse.Data
                    ?.FirstOrDefault(p => p.ProductId == product.Id.Value);
                quantityInStock = productInStock?.QuantityInStock ?? 0;
            }

            var convertedPrice = _currencyConverter
                .Convert(product.UnitPrice.Amount, query.CurrencyCode);

            productsViewModel.Add(new ProductViewModel(
                product.Id.Value,
                product.Name,
                product.Category,
                product.Description,
                product.ImageUrl,
                Math.Round(convertedPrice, 2).ToString(),
                currency.Symbol,
                quantityInStock
            ));
        }

        return productsViewModel;
    }
}

public record class CheckProductsInStockRequest(
    Guid[] ProductIds);

public record class InventoryStockUnitViewModel(
    Guid InventoryStockUnitId,
    Guid ProductId,
    int QuantityInStock);