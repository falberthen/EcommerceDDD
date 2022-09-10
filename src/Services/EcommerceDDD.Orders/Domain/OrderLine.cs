using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Orders.Domain;

public class OrderLine : ValueObject<OrderLine>
{
    public ProductItem ProductItem { get; private set; }

    internal OrderLine(ProductItem productItem)
    {
        if (productItem.ProductId == null)
            throw new DomainException("ProductId is required.");

        if (string.IsNullOrEmpty(productItem.ProductName))
            throw new DomainException("Product name is required.");
        
        if (productItem.UnitPrice == null)
            throw new DomainException("Product unit price is required.");
        
        if (productItem.Quantity <= 0)
            throw new DomainException("Product quantity must be > 0.");

        ProductItem = productItem;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductItem;
    }
}