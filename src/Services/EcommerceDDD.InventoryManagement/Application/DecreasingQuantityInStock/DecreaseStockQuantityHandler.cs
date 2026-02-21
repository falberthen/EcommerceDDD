namespace EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

public class DecreaseStockQuantityHandler(
	IQuerySessionWrapper querySession,
	IEventStoreRepository<InventoryStockUnit> inventoryStockUnitWriteRepository
) : ICommandHandler<DecreaseStockQuantity>
{
	private readonly IQuerySessionWrapper _querySession = querySession;
	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = inventoryStockUnitWriteRepository;

	public async Task<Result> HandleAsync(DecreaseStockQuantity command, CancellationToken cancellationToken)
	{
		var query = await _querySession.QueryFirstOrDefaultAsync<InventoryStockUnitDetails>(
			x => x.ProductId == command.ProductId.Value, cancellationToken);

		if (query is null)
			return Result.Fail($"The product {command.ProductId.Value} was not found in the inventory.");

		Guid inventoryStockUnitId = query.Id;
		var inventoryStockUnit = await _inventoryStockUnitWriteRepository
			.FetchStreamAsync(inventoryStockUnitId, cancellationToken: cancellationToken);

		if (inventoryStockUnit is null)
			return Result.Fail($"The inventory stock unit {inventoryStockUnitId} was not found.");

		inventoryStockUnit.DecreaseStockQuantity(command.QuantityDecreased);

		await _inventoryStockUnitWriteRepository
			.AppendEventsAsync(inventoryStockUnit);

		return Result.Ok();
	}
}
