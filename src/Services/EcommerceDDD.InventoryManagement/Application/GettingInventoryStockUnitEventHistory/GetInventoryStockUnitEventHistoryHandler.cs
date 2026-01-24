namespace EcommerceDDD.InventoryManagement.Application.GettingInventoryStockUnitEventHistory;

public class GetInventoryStockUnitEventHistoryHandler(
    IQuerySession querySession
) : IQueryHandler<GetInventoryStockUnitEventHistory, IReadOnlyList<InventoryStockUnitEventHistory>> 
{
    private readonly IQuerySession _querySession = querySession
		?? throw new ArgumentNullException(nameof(querySession));

	public async Task<IReadOnlyList<InventoryStockUnitEventHistory>> HandleAsync(GetInventoryStockUnitEventHistory query,
        CancellationToken cancellationToken)
    {
        var stockUnitEventStory = await _querySession.Query<InventoryStockUnitEventHistory>()
           .Where(c => c.EventData.Contains(query.ProductId.Value.ToString()))
           .OrderBy(c => c.Timestamp)
           .ToListAsync(cancellationToken);

        return stockUnitEventStory;
    }
}
