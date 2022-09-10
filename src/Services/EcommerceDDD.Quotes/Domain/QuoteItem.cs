using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Domain;

public class QuoteItem : ValueObject<QuoteItem>
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    internal QuoteItem(ProductId productId, int quantity)
    {
        if (productId == null)
            throw new DomainException("ProductId is required");

        if (quantity <= 0)
            throw new DomainException("Product quantity must be > 0.");

        ProductId = productId;
        Quantity = quantity;
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity == 0)
            throw new DomainException("The product quantity must be at last 1.");

        Quantity = quantity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return Quantity;
    }
}