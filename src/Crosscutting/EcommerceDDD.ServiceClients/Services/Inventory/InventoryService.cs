namespace EcommerceDDD.ServiceClients.Services.Inventory;

public class InventoryService(InventoryManagementClient inventoryManagementClient) : IInventoryService
{
	private readonly InventoryManagementClient _inventoryManagementClient = inventoryManagementClient;

	public async Task<List<InventoryStockUnitViewModel>?> CheckStockQuantityAsync(List<Guid?> productIds, CancellationToken cancellationToken)
	{
		var request = new CheckProductsInStockRequest()
		{
			ProductIds = productIds
		};

		return await _inventoryManagementClient.Api.V2.Internal.Inventory.CheckStockQuantity
			.PostAsync(request, cancellationToken: cancellationToken);
	}

	public async Task<bool> DecreaseStockQuantityAsync(Guid productId, int quantity, Guid orderId, CancellationToken cancellationToken)
	{
		var request = new DecreaseQuantityInStockRequest()
		{
			DecreasedQuantity = quantity,
			OrderId = orderId
		};

		try
		{
			await _inventoryManagementClient.Api.V2.Internal.Inventory[productId].DecreaseStockQuantity
				.PutAsync(request, cancellationToken: cancellationToken);
			return true;
		}
		// 422 is the inventory's business rule (not enough stock), not a technical failure to retry.
		catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.UnprocessableEntity)
		{
			return false;
		}
	}

	public async Task IncreaseStockQuantityAsync(Guid productId, int quantity, Guid orderId, CancellationToken cancellationToken)
	{
		var request = new IncreaseQuantityInStockRequest()
		{
			IncreasedQuantity = quantity,
			OrderId = orderId
		};

		await _inventoryManagementClient.Api.V2.Internal.Inventory[productId].IncreaseStockQuantity
			.PutAsync(request, cancellationToken: cancellationToken);
	}
}
