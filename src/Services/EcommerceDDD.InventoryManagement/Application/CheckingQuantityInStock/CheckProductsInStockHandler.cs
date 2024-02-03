namespace EcommerceDDD.InventoryManagement.Application.CheckingQuantityInStock;

public class CheckProductsInStockHandler : 
    IQueryHandler<CheckProductsInStock, IList<InventoryStockUnitViewModel>>
{
    private readonly IQuerySession _querySession;

    public CheckProductsInStockHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<IList<InventoryStockUnitViewModel>> Handle(CheckProductsInStock query,
        CancellationToken cancellationToken)
    {
        var viewModels = new List<InventoryStockUnitViewModel>();

        if (!query.ProductIds.Any())
            throw new ApplicationLogicException("The list of Products to check cannot be empty.");

        IList<Guid> productIds = query.ProductIds
            .Select(p => p.Value).Distinct().ToList();
        var projectedInventoryStockUnits = _querySession.Query<InventoryStockUnitDetails>()
            .Where(x => productIds.Contains(x.ProductId)).ToList();

        foreach (var inventoryStockUnit in projectedInventoryStockUnits)
        {
            viewModels.Add(new InventoryStockUnitViewModel(
                inventoryStockUnit.Id,
                inventoryStockUnit.ProductId,
                inventoryStockUnit.QuantityInStock));
        }

        return await Task.FromResult(viewModels);
    }
}
