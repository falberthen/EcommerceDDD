namespace EcommerceDDD.InventoryManagement.Domain.Events;

public record class StockQuantityIncreased(
    Guid InventoryStockUnitId,
    Guid ProductId,
    int QuantityIncreased) : DomainEvent;
