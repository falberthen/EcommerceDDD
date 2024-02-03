namespace EcommerceDDD.InventoryManagement.Domain;

public class InventoryStockUnit : AggregateRoot<InventoryStockUnitId>
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    public static InventoryStockUnit EnterStockUnit(ProductId productId, int initialQuantity)
    {
        return new InventoryStockUnit(productId, initialQuantity);
    }

    public void DecreaseStockQuantity(int quantityToDecrease)
    {
        if (quantityToDecrease <= 0)
            throw new BusinessRuleException("Quantity to decrease must be greater than zero.");

        if (Quantity < quantityToDecrease)
            throw new BusinessRuleException("Insufficient quantity in stock.");

        var @event = StockQuantityDecreased.Create(
            Id.Value,
            ProductId.Value,
            quantityToDecrease);
        
        AppendEvent(@event);
        Apply(@event);
    }

    public void IncreaseStockQuantity(int quantityToIncrease)
    {
        if (quantityToIncrease <= 0)
            throw new BusinessRuleException("Quantity to increase must be greater than zero.");

        var @event = StockQuantityIncreased.Create(
            Id.Value,
            ProductId.Value,
            quantityToIncrease);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(UnitEnteredInStock @event)
    {
        Id = InventoryStockUnitId.Of(@event.InventoryStockUnitId);
        ProductId = ProductId.Of(@event.ProductId);
        Quantity = @event.InitialQuantity;
    }

    private void Apply(StockQuantityDecreased @event)
    {
        Quantity -= @event.QuantityDecreased;
    }

    private void Apply(StockQuantityIncreased @event)
    {
        Quantity += @event.QuantityIncreased;
    }

    private InventoryStockUnit(ProductId productId, int initialQuantity)
    {
        if (initialQuantity < 0)
            throw new BusinessRuleException("Initial quantity cannot be less than zero.");
        
        var @event = UnitEnteredInStock.Create(
            Guid.NewGuid(),
            productId.Value,
            initialQuantity);
    
        AppendEvent(@event);
        Apply(@event);
    }

    private InventoryStockUnit() { }
}