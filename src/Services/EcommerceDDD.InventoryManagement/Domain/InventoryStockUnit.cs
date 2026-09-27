namespace EcommerceDDD.InventoryManagement.Domain;

public class InventoryStockUnit : AggregateRoot<InventoryStockUnitId>
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    private HashSet<Guid> _decreasedForOrders = default!;
    private HashSet<Guid> _restockedForOrders = default!;

    public static InventoryStockUnit EnterStockUnit(ProductId productId, int initialQuantity) =>
        new InventoryStockUnit(productId, initialQuantity);

    /// <summary>
    /// Takes stock for an order. A repeated call for the same order is ignored.
    /// </summary>
    public void DecreaseStockQuantity(int quantityToDecrease, Guid orderId)
    {
        if (_decreasedForOrders.Contains(orderId))
            return;

        if (quantityToDecrease <= 0)
            throw new DomainException("Quantity to decrease must be greater than zero.");

        if (Quantity < quantityToDecrease)
            throw new DomainException("Insufficient quantity in stock.");

        var @event = new StockQuantityDecreased(
            Id.Value,
            ProductId.Value,
            quantityToDecrease,
            orderId);
        
        AppendEvent(@event);
        Apply(@event);
    }

    /// <summary>
    /// Returns to stock what an order took. Ignored when the order took nothing from this unit,
    /// or when it was already returned.
    /// </summary>
    public void IncreaseStockQuantity(int quantityToIncrease, Guid orderId)
    {
        if (!_decreasedForOrders.Contains(orderId) || _restockedForOrders.Contains(orderId))
            return;

        if (quantityToIncrease <= 0)
            throw new DomainException("Quantity to increase must be greater than zero.");

        var @event = new StockQuantityIncreased(
            Id.Value,
            ProductId.Value,
            quantityToIncrease,
            orderId);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Apply(UnitEnteredInStock @event)
    {
        Id = InventoryStockUnitId.Of(@event.InventoryStockUnitId);
        ProductId = ProductId.Of(@event.ProductId);
        Quantity = @event.InitialQuantity;
        _decreasedForOrders = new();
        _restockedForOrders = new();
    }

    public void Apply(StockQuantityDecreased @event)
    {
        Quantity -= @event.QuantityDecreased;
        _decreasedForOrders.Add(@event.OrderId);
    }

    public void Apply(StockQuantityIncreased @event)
    {
        Quantity += @event.QuantityIncreased;
        _restockedForOrders.Add(@event.OrderId);
    }

    private InventoryStockUnit(ProductId productId, int initialQuantity)
    {
        if (initialQuantity < 0)
            throw new DomainException("Initial quantity cannot be less than zero.");
        
        var @event = new UnitEnteredInStock(
            Guid.NewGuid(),
            productId.Value,
            initialQuantity);
    
        AppendEvent(@event);
        Apply(@event);
    }

    private InventoryStockUnit() { }
}