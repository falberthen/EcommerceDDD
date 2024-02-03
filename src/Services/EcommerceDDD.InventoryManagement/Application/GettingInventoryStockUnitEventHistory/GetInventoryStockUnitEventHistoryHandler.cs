namespace EcommerceDDD.InventoryManagement.Application.GettingInventoryStockUnitEventHistory;

public class GetInventoryStockUnitEventHistoryHandler : 
    IQueryHandler<GetInventoryStockUnitEventHistory, IList<InventoryStockUnitEventHistory>> 
{
    private readonly IQuerySession _querySession;

    public GetInventoryStockUnitEventHistoryHandler(
        IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<IList<InventoryStockUnitEventHistory>> Handle(GetInventoryStockUnitEventHistory query,
        CancellationToken cancellationToken)
    {
        var stockUnitEventStory = await _querySession.Query<InventoryStockUnitEventHistory>()
           .Where(c => c.EventData.Contains(query.ProductId.Value.ToString()))
           .OrderBy(c => c.Timestamp)
           .ToListAsync(cancellationToken);

        return stockUnitEventStory.ToList();
    }
}
