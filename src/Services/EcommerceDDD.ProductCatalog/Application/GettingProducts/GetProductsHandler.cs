namespace EcommerceDDD.ProductCatalog.Application.GettingProducts;

public class GetProductsHandler(
	IInventoryService inventoryService,
	IProducts productsRepository,
	ICurrencyConverter currencyConverter
) : IQueryHandler<GetProducts, IList<ProductViewModel>>
{
	private readonly IInventoryService _inventoryService = inventoryService;
	private readonly IProducts _productsRepository = productsRepository;
	private readonly ICurrencyConverter _currencyConverter = currencyConverter;

	public async Task<Result<IList<ProductViewModel>>> HandleAsync(GetProducts query, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(query.CurrencyCode))
			return Result.Fail<IList<ProductViewModel>>("Currency code cannot be empty.");

		var productsViewModel = new List<ProductViewModel>();
		var products = query.ProductIds.Count == 0
			? await _productsRepository.ListAll(cancellationToken)
			: await _productsRepository.GetByIds(query.ProductIds, cancellationToken);

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
				ProductId: product.Id.Value,
				Name: product.Name,
				Category: product.Category,
				Description: product.Description,
				ImageUrl: product.ImageUrl,
				Price: Math.Round(convertedPrice, 2),
				CurrencySymbol: currency.Symbol,
				QuantityInStock: quantityInStock
			));
		}

		return Result.Ok<IList<ProductViewModel>>(productsViewModel);
	}

	private async Task<List<InventoryStockUnitViewModel>> GetProductsFromInventoryAsync(List<Guid?> productIds,
		CancellationToken cancellationToken)
	{
		try
		{
			var response = await _inventoryService
				.CheckStockQuantityAsync(productIds, cancellationToken);

			return response ?? new List<InventoryStockUnitViewModel>();
		}
		catch (Exception)
		{
			return new List<InventoryStockUnitViewModel>();
		}
	}
}
