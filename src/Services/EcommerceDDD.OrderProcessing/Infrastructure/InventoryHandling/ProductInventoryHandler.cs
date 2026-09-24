namespace EcommerceDDD.OrderProcessing.Infrastructure.InventoryHandling;

public class ProductInventoryHandler(
	IInventoryService inventoryService,
	ILogger<ProductInventoryHandler> logger) : IProductInventoryHandler
{
	private readonly IInventoryService _inventoryService = inventoryService;
	private readonly ILogger<ProductInventoryHandler> _logger = logger;

	public async Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken)
	{
		var productIds = items
			.Select(p => new Guid?(p.ProductId.Value))
			.ToList();

		var response = await _inventoryService
			.CheckStockQuantityAsync(productIds, cancellationToken);

		if (response is null)
		{
			_logger.LogWarning("Stock check returned no data for {Count} product(s); treating as out of stock.", items.Count);
			return false;
		}

		var shortItems = items.Where(item =>
		{
			var stock = response.SingleOrDefault(p => p.ProductId == item.ProductId.Value);
			return stock is null || item.Quantity > stock.QuantityInStock;
		}).ToList();

		if (shortItems.Count > 0)
		{
			foreach (var item in shortItems)
			{
				var stock = response.SingleOrDefault(p => p.ProductId == item.ProductId.Value);
				_logger.LogWarning(
					"Insufficient stock for product {ProductId}: wanted {Wanted}, available {Available}.",
					item.ProductId.Value, item.Quantity, stock?.QuantityInStock ?? 0);
			}
			return false;
		}

		return true;
	}

	public async Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken)
	{
		var tasks = items.Select(item =>
			_inventoryService.DecreaseStockQuantityAsync(item.ProductId.Value, item.Quantity, cancellationToken));

		await Task.WhenAll(tasks);

		foreach (var item in items)
			_logger.LogInformation(
				"Stock decremented for product {ProductId} by {Quantity}.",
				item.ProductId.Value, item.Quantity);
	}

	public async Task IncreaseQuantityInStockAsync(IReadOnlyList<ProductItemData> items, CancellationToken cancellationToken)
	{
		var tasks = items.Select(item =>
			_inventoryService.IncreaseStockQuantityAsync(item.ProductId.Value, item.Quantity, cancellationToken));

		await Task.WhenAll(tasks);

		foreach (var item in items)
			_logger.LogInformation(
				"Stock restored for product {ProductId} by {Quantity}.",
				item.ProductId.Value, item.Quantity);
	}
}
