using EcommerceDDD.ServiceClients.InventoryManagement;
using EcommerceDDD.ServiceClients.InventoryManagement.Models;

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

    public async Task DecreaseStockQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var request = new DecreaseQuantityInStockRequest()
        {
            DecreasedQuantity = quantity
        };

        await _inventoryManagementClient.Api.V2.Internal.Inventory[productId].DecreaseStockQuantity
            .PutAsync(request, cancellationToken: cancellationToken);
    }
}
