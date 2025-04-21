using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.ProductCatalog.Application.Products.GettingProducts;

public class GetProductsHandler(
	ApiGatewayClient apiGatewayClient,
	IProducts productsRepository,
	ICurrencyConverter currencyConverter
) : IQueryHandler<GetProducts, IList<ProductViewModel>> 
{
	private readonly ApiGatewayClient _apiGatewayClient = apiGatewayClient;
	private readonly IProducts _productsRepository = productsRepository;
	private readonly ICurrencyConverter _currencyConverter = currencyConverter;

	public async Task<IList<ProductViewModel>> HandleAsync(GetProducts query, CancellationToken cancellationToken)
    {
        var productsViewModel = new List<ProductViewModel>();
        var products = query.ProductIds.Count == 0
            ? await _productsRepository.ListAll(cancellationToken)
            : await _productsRepository.GetByIds(query.ProductIds);

        if (string.IsNullOrEmpty(query.CurrencyCode))
            throw new RecordNotFoundException("Currency code cannot be empty.");

        // Getting stock quantity
        var productIds = products.Select(x => 
			new Guid?(x.Id.Value)).ToList();

		var inventoryResponse = await GetProductsFromInventoryAsync(productIds, cancellationToken);
		var currency = Currency.OfCode(query.CurrencyCode);

		foreach (var product in products)
		{
			int quantityInStock = 0;
			if (inventoryResponse?.Count > 0)
			{
				var productInStock = inventoryResponse
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
				Math.Round(convertedPrice, 2),
				currency.Symbol,
				quantityInStock
			));
		}

		return productsViewModel;
    }

	private async Task<List<InventoryStockUnitViewModel>> GetProductsFromInventoryAsync(List<Guid?> productIds, 
		CancellationToken cancellationToken)
	{
		try
		{
			var request = new CheckProductsInStockRequest()
			{
				ProductIds = productIds
			};
			var inventoryRequestBuilder = _apiGatewayClient.Api.Inventory;
			var response = await inventoryRequestBuilder.CheckStockQuantity
				.PostAsync(request, cancellationToken: cancellationToken);

			if (response?.Data is null)
				throw new ApplicationLogicException(response?.Message ?? string.Empty);

			return response.Data;
		}
		catch (Exception ex)
		{
			return new List<InventoryStockUnitViewModel>();
		}		
	}
}