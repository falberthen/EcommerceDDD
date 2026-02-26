using EcommerceDDD.ServiceClients.InventoryManagement.Models;

namespace EcommerceDDD.ServiceClients.Services.Inventory;

public interface IInventoryService
{
    Task<List<InventoryStockUnitViewModel>?> CheckStockQuantityAsync(List<Guid?> productIds, CancellationToken cancellationToken);
    Task DecreaseStockQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken);
}
