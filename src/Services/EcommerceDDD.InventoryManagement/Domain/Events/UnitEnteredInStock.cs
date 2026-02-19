namespace EcommerceDDD.InventoryManagement.Domain.Events;

public record class UnitEnteredInStock(
    Guid InventoryStockUnitId,
    Guid ProductId,
    int InitialQuantity) : DomainEvent;
