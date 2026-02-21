namespace EcommerceDDD.OrderProcessing.Domain;

public class ProductItem : ValueObject<ProductItem>
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public Currency Currency { get; private set; }
    public int Quantity { get; private set; }

    internal ProductItem(
        ProductId productId, string productName, Money unitPrice, int quantity, Currency currency)
    {
        if (productId is null)
            throw new DomainException("ProductId is required.");
        if (string.IsNullOrEmpty(productName))
            throw new DomainException("Product name cannot be null or empty.");
        if (unitPrice is null)
            throw new DomainException("Product unit price is required.");
        if (currency is null)
            throw new DomainException("Currency is required.");
        if (quantity <= 0)
            throw new DomainException("Product quantity must be > 0.");

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
        yield return Currency;
        yield return Quantity;
    }
}