using EcommerceDDD.InventoryManagement.Application.IncreaseQuantityInStock;

namespace EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

public class IncreaseQuantityInStockHandler(
	IQuerySessionWrapper querySession,
    IEventStoreRepository<InventoryStockUnit> inventoryStockUnitWriteRepository
)
{
	private readonly IQuerySessionWrapper _querySession = querySession;
	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = inventoryStockUnitWriteRepository;

	public async Task<Result> HandleAsync(IncreaseStockQuantity command, CancellationToken cancellationToken)
    {
        var existingEntry = await _querySession.QueryFirstOrDefaultAsync<InventoryStockUnitDetails>(
            x => x.ProductId == command.ProductId.Value, cancellationToken);

        if (existingEntry is null)
            return Result.Fail($"The product {command.ProductId.Value} was not found in the inventory.");

        Guid inventoryStockUnitId = existingEntry.Id;
		var inventoryStockUnit = await _inventoryStockUnitWriteRepository
			.FetchForWritingAsync(inventoryStockUnitId, cancellationToken: cancellationToken);

		if (inventoryStockUnit is null)
            return Result.Fail($"The inventory stock unit {inventoryStockUnitId} was not found.");

        inventoryStockUnit.IncreaseStockQuantity(command.QuantityIncreased);

        await _inventoryStockUnitWriteRepository
			.AppendEventsAndCommitAsync(inventoryStockUnit, cancellationToken: cancellationToken);

		return Result.Ok();
    }
}
