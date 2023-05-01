namespace EcommerceDDD.Quotes.Domain;

public class QuoteItem
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    internal static QuoteItem Create(ProductId productId, int quantity)
    {
        if (productId is null)
            throw new BusinessRuleException("ProductId is required");

        if (quantity <= 0)
            throw new BusinessRuleException("Product quantity must be > 0.");

        return new QuoteItem(productId, quantity);
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity == 0)
            throw new BusinessRuleException("The product quantity must be at last 1.");

        Quantity = quantity;
    }

    private QuoteItem(ProductId productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}