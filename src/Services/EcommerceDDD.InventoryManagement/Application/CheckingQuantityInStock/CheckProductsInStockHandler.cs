namespace EcommerceDDD.InventoryManagement.Application.CheckingQuantityInStock;

public class CheckProductsInStockHandler(
	IQuerySessionWrapper querySession
) : IQueryHandler<CheckProductsInStock, IList<InventoryStockUnitViewModel>>
{
    private readonly IQuerySessionWrapper _querySession = querySession;

    public async Task<Result<IList<InventoryStockUnitViewModel>>> HandleAsync(CheckProductsInStock query,
        CancellationToken cancellationToken)
    {
        if (!query.ProductIds.Any())
            return Result.Fail<IList<InventoryStockUnitViewModel>>("The list of Products to check cannot be empty.");

        var viewModels = new List<InventoryStockUnitViewModel>();

        IList<Guid> productIds = query.ProductIds
            .Select(p => p.Value).Distinct().ToList();
        IReadOnlyList<InventoryStockUnitDetails> projectedInventoryStockUnits = await _querySession.QueryListAsync<InventoryStockUnitDetails>(
            x => productIds.Contains(x.ProductId), cancellationToken);

        foreach (var inventoryStockUnit in projectedInventoryStockUnits)
        {
            viewModels.Add(new InventoryStockUnitViewModel(
                inventoryStockUnit.Id,
                inventoryStockUnit.ProductId,
                inventoryStockUnit.QuantityInStock));
        }

        return Result.Ok<IList<InventoryStockUnitViewModel>>(viewModels);
    }
}
