namespace EcommerceDDD.InventoryManagement.Application.CheckingQuantityInStock;

public record class InventoryStockUnitViewModel(
    Guid InventoryStockUnitId,
    Guid ProductId, 
    int QuantityInStock);