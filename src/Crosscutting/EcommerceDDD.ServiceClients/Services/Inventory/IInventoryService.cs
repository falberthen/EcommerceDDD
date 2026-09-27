namespace EcommerceDDD.ServiceClients.Services.Inventory;

public interface IInventoryService
{
	Task<List<InventoryStockUnitViewModel>?> CheckStockQuantityAsync(List<Guid?> productIds, CancellationToken cancellationToken);
	Task<bool> DecreaseStockQuantityAsync(Guid productId, int quantity, Guid orderId, CancellationToken cancellationToken);
	Task IncreaseStockQuantityAsync(Guid productId, int quantity, Guid orderId, CancellationToken cancellationToken);
}
