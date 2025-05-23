﻿namespace EcommerceDDD.InventoryManagement.Application.GettingInventoryStockUnitEventHistory;

public class GetInventoryStockUnitEventHistoryHandler(
    IQuerySession querySession
) : IQueryHandler<GetInventoryStockUnitEventHistory, IList<InventoryStockUnitEventHistory>> 
{
    private readonly IQuerySession _querySession = querySession;

    public async Task<IList<InventoryStockUnitEventHistory>> HandleAsync(GetInventoryStockUnitEventHistory query,
        CancellationToken cancellationToken)
    {
        var stockUnitEventStory = await _querySession.Query<InventoryStockUnitEventHistory>()
           .Where(c => c.EventData.Contains(query.ProductId.Value.ToString()))
           .OrderBy(c => c.Timestamp)
           .ToListAsync(cancellationToken);

        return stockUnitEventStory.ToList();
    }
}
