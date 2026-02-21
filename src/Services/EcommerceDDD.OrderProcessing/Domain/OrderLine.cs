namespace EcommerceDDD.OrderProcessing.Domain;

public class OrderLine
{
    public ProductItem ProductItem { get; private set; }

    internal static OrderLine Create(ProductItem productItem)
    {
        if (productItem.ProductId is null)
            throw new DomainException("ProductId is required.");

        if (string.IsNullOrEmpty(productItem.ProductName))
            throw new DomainException("Product name is required.");
        
        if (productItem.UnitPrice is null)
            throw new DomainException("Product unit price is required.");
        
        if (productItem.Quantity <= 0)
            throw new DomainException("Product quantity must be > 0.");

        return new OrderLine(productItem);
    }

    private OrderLine(ProductItem productItem) => ProductItem = productItem;
}