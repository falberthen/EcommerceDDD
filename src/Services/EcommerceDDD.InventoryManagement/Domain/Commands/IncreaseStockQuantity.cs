namespace EcommerceDDD.InventoryManagement.Domain.Commands;

public record class IncreaseStockQuantity : ICommand
{
    public ProductId ProductId { get; private set; }
    public int QuantityIncreased { get; private set; }

    public static IncreaseStockQuantity Create(
       ProductId productId,
       int quantityIncreased)
    {
        if (productId is null)
            throw new ArgumentNullException(nameof(productId));
        if (quantityIncreased < 1)
            throw new ArgumentNullException(nameof(quantityIncreased));

        return new IncreaseStockQuantity(productId, quantityIncreased);
    }

    private IncreaseStockQuantity(ProductId productId, int quantityIncreased)
    {
        ProductId = productId;
        QuantityIncreased = quantityIncreased;
    }
}