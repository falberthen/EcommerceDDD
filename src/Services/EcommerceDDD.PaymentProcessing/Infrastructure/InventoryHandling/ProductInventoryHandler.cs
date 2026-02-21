namespace EcommerceDDD.PaymentProcessing.Infrastructure.InventoryHandling;

public class ProductInventoryHandler(InventoryManagementClient inventoryManagementClient) : IProductInventoryHandler
{
	private readonly InventoryManagementClient _inventoryManagementClient = inventoryManagementClient;

	public async Task<bool> CheckProductsInStockAsync(IReadOnlyList<ProductItem> productItems, CancellationToken cancellationToken)
	{
		var productIdFilter = productItems
			.Select(p => new Guid?(p.ProductId.Value))
			.ToList();

		var request = new CheckProductsInStockRequest()
		{
			ProductIds = productIdFilter
		};

		try
		{
			var inventoryRequestBuilder = _inventoryManagementClient.Api.V2.Inventory;
			var response = await inventoryRequestBuilder
				.CheckStockQuantity
				.PostAsync(request, cancellationToken: cancellationToken);

			if (response is null)
				return false;

			var inventoryData = response;

			bool hasOutOfStockItem = productItems.Any(item =>
			{
				var stock = inventoryData
					.SingleOrDefault(p => p.ProductId == item.ProductId.Value);

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
		var tasks = productItems.Select(async productItem =>
		{
			var request = new DecreaseQuantityInStockRequest
			{
				DecreasedQuantity = productItem.Quantity
			};

			var inventoryRequestBuilder = _inventoryManagementClient.Api.V2.Inventory[productItem.ProductId.Value];
			await inventoryRequestBuilder
				.DecreaseStockQuantity
				.PutAsync(request, cancellationToken: cancellationToken);
		});

		await Task.WhenAll(tasks);
	}
}
