namespace EcommerceDDD.InventoryManagement.Domain.Events;

public record class StockQuantityDecreased(
    Guid InventoryStockUnitId,
    Guid ProductId,
    int QuantityDecreased) : DomainEvent;
