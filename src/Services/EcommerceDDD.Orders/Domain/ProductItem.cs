namespace EcommerceDDD.Orders.Domain;

public class ProductItem : ValueObject<ProductItem>
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    internal ProductItem(ProductId productId, string productName, Money unitPrice, int quantity)
    {
        if (productId is null)
            throw new BusinessRuleException("ProductId is required.");

        if (string.IsNullOrEmpty(productName))
            throw new BusinessRuleException("Product name cannot be null or empty.");

        if (unitPrice is null)
            throw new BusinessRuleException("Product unit price is required.");

        if (quantity <= 0)
            throw new BusinessRuleException("Product quantity must be > 0.");

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
        yield return Quantity;
    }
}