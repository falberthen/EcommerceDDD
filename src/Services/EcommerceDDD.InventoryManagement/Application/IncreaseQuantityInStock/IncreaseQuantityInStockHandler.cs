namespace EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

public class IncreaseQuantityInStockHandler(
	IQuerySessionWrapper querySession,
    IEventStoreRepository<InventoryStockUnit> inventoryStockUnitWriteRepository
) : ICommandHandler<IncreaseStockQuantity>
{
    private readonly IQuerySessionWrapper _querySession = querySession;
    private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = inventoryStockUnitWriteRepository;

    public async Task HandleAsync(IncreaseStockQuantity command, CancellationToken cancellationToken)
    {
        // Check if the product is already in stock
        var existingEntry = _querySession.Query<InventoryStockUnitDetails>()
            .Where(x => x.ProductId == command.ProductId.Value)
            .FirstOrDefault() 
            ?? throw new RecordNotFoundException(
                $"The product {command.ProductId.Value} was not found in the inventory.");

        Guid inventoryStockUnitId = existingEntry.Id;
        var inventoryStockUnit = await _inventoryStockUnitWriteRepository
            .FetchStreamAsync(inventoryStockUnitId)
            ?? throw new RecordNotFoundException(
                $"The inventory stock unit {inventoryStockUnitId} was not found.");

        inventoryStockUnit.IncreaseStockQuantity(command.QuantityIncreased);

        await _inventoryStockUnitWriteRepository
            .AppendEventsAsync(inventoryStockUnit);
    }
}