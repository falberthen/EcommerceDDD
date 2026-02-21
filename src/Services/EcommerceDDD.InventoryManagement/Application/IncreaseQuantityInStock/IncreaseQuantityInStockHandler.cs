using EcommerceDDD.InventoryManagement.Application.IncreaseQuantityInStock;

namespace EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

public class IncreaseQuantityInStockHandler(
	IQuerySessionWrapper querySession,
    IEventStoreRepository<InventoryStockUnit> inventoryStockUnitWriteRepository
) : ICommandHandler<IncreaseStockQuantity>
{
	private readonly IQuerySessionWrapper _querySession = querySession;
	private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = inventoryStockUnitWriteRepository;

	public async Task<Result> HandleAsync(IncreaseStockQuantity command, CancellationToken cancellationToken)
    {
        var existingEntry = _querySession.Query<InventoryStockUnitDetails>()
            .Where(x => x.ProductId == command.ProductId.Value)
            .FirstOrDefault();

        if (existingEntry is null)
            return Result.Fail($"The product {command.ProductId.Value} was not found in the inventory.");

        Guid inventoryStockUnitId = existingEntry.Id;
		var inventoryStockUnit = await _inventoryStockUnitWriteRepository
			.FetchStreamAsync(inventoryStockUnitId, cancellationToken: cancellationToken);

		if (inventoryStockUnit is null)
            return Result.Fail($"The inventory stock unit {inventoryStockUnitId} was not found.");

        inventoryStockUnit.IncreaseStockQuantity(command.QuantityIncreased);

        await _inventoryStockUnitWriteRepository
			.AppendEventsAsync(inventoryStockUnit, cancellationToken: cancellationToken);

		return Result.Ok();
    }
}
