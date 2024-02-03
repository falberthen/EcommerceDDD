namespace EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

public class IncreaseQuantityInStockHandler : ICommandHandler<IncreaseStockQuantity>
{
    private readonly IQuerySession _querySession;
    private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository;

    public IncreaseQuantityInStockHandler(
        IQuerySession querySession,
        IEventStoreRepository<InventoryStockUnit> inventoryStockUnitWriteRepository
        )
    {
        _querySession = querySession;
        _inventoryStockUnitWriteRepository = inventoryStockUnitWriteRepository;
    }

    public async Task Handle(IncreaseStockQuantity command, CancellationToken cancellationToken)
    {
        // Check if the product is already in stock
        var existingEntry = await _querySession.Query<InventoryStockUnitDetails>()
            .Where(x => x.ProductId == command.ProductId.Value)
            .FirstOrDefaultAsync() 
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