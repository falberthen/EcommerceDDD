using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Quotes.Domain;

public record class QuoteItem
{
    public Guid Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    internal QuoteItem(ProductId productId, int quantity)
    {
        if (productId == null)
            throw new DomainException("ProductId is required");

        if (quantity <= 0)
            throw new DomainException("Product quantity must be > 0.");

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
    }

    internal void ChangeQuantity(int quantity)
    {
        if (quantity == 0)
            throw new DomainException("The product quantity must be at last 1.");

        Quantity = quantity;
    }
}