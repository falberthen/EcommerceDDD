namespace EcommerceDDD.InventoryManagement.Application.GettingInventoryStockUnitEventHistory;

public record InventoryStockUnitEventHistory(
    Guid Id, 
    Guid AggregateId,
    string EventTypeName,
    string EventData,
    DateTime Timestamp) : IEventHistory
{
    public static InventoryStockUnitEventHistory Create(IEvent @event, Guid aggregateId)
    {
        var serialized = JsonConvert.SerializeObject(@event.Data);
        return new InventoryStockUnitEventHistory(
            Guid.NewGuid(), 
            aggregateId, 
            @event.EventTypeName, 
            serialized,
            DateTime.UtcNow);
    }
}