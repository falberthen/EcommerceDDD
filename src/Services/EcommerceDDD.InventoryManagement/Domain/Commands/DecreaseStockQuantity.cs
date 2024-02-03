namespace EcommerceDDD.InventoryManagement.Domain.Commands;

public record class DecreaseStockQuantity : ICommand
{
    public ProductId ProductId { get; private set; }
    public int QuantityDecreased { get; private set; }

    public static DecreaseStockQuantity Create(
       ProductId productId,
       int quantityDecreased)
    {
        if (productId is null)
            throw new ArgumentNullException(nameof(productId));
        if (quantityDecreased <= 0)
            throw new ArgumentNullException(nameof(quantityDecreased));

        return new DecreaseStockQuantity(productId, quantityDecreased);
    }

    private DecreaseStockQuantity(ProductId productId, int quantityDecreased)
    {
        ProductId = productId;
        QuantityDecreased = quantityDecreased;
    }
}