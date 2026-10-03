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
			return Result.Fail(new RecordNotFoundError($"The product {command.ProductId.Value} was not found in the inventory."));

		Guid inventoryStockUnitId = existingEntry.Id;
		var inventoryStockUnit = await _inventoryStockUnitWriteRepository
			.FetchForWritingAsync(inventoryStockUnitId, cancellationToken: cancellationToken);

		if (inventoryStockUnit is null)
			throw new RecordNotFoundException($"The inventory stock unit {inventoryStockUnitId} was not found.");

		inventoryStockUnit.IncreaseStockQuantity(command.QuantityIncreased, command.OrderId);

		await _inventoryStockUnitWriteRepository
			.AppendEventsAndCommitAsync(inventoryStockUnit, cancellationToken: cancellationToken);

		return Result.Ok();
	}
}
