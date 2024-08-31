namespace EcommerceDDD.InventoryManagement.Application.EnteringProductInStock;

public class EnterProductInStockHandler(
    IQuerySession querySession,
    IEventStoreRepository<InventoryStockUnit> inventoryStockUnitWriteRepository
        ) : ICommandHandler<EnterProductInStock>
{
    private readonly IQuerySession _querySession = querySession;
    private readonly IEventStoreRepository<InventoryStockUnit> _inventoryStockUnitWriteRepository = inventoryStockUnitWriteRepository;

    public async Task Handle(EnterProductInStock command, CancellationToken cancellationToken)
    {
        // Check if the product is already in stock
        var productIds = command.ProductIdsQuantities
            .Select(pid => pid.Item1.Value).ToList();

        var existingEntries = await _querySession.Query<InventoryStockUnitDetails>()
            .Where(x => productIds.Contains(x.ProductId))
            .ToListAsync();

        if (!existingEntries.Any())
        {
            foreach (var productQuantity in command.ProductIdsQuantities)
            {
                var inventoryStockUnit = InventoryStockUnit.EnterStockUnit(
                    productQuantity.Item1, productQuantity.Item2);

                await _inventoryStockUnitWriteRepository
                    .AppendEventsAsync(inventoryStockUnit);
            }
        }
    }
}