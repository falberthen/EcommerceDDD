namespace EcommerceDDD.PaymentProcessing.Infrastructure.InventoryHandling;

public class ProductInventoryHandler(IInventoryService inventoryService) : IProductInventoryHandler
{
	private readonly IInventoryService _inventoryService = inventoryService;

	public async Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken)
	{
		var productIds = productItems
			.Select(p => new Guid?(p.ProductId.Value))
			.ToList();

		try
		{
			var response = await _inventoryService
				.CheckStockQuantityAsync(productIds, cancellationToken);

			if (response is null)
				return false;

			bool hasOutOfStockItem = productItems.Any(item =>
			{
				var stock = response.SingleOrDefault(p => p.ProductId == item.ProductId.Value);
				if (stock is null) return true; // treat missing as out of stock
				return item.Quantity > stock.QuantityInStock;
			});

			return !hasOutOfStockItem;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public async Task DecreaseQuantityInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken)
	{
		var tasks = productItems.Select(productItem =>
			_inventoryService.DecreaseStockQuantityAsync(productItem.ProductId.Value, productItem.Quantity, cancellationToken));

		await Task.WhenAll(tasks);
	}
}
