using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Quotes;

public record class QuoteItemProductData
{
    public ProductId ProductId { get; init; }
    public Money ProductPrice { get; init; }
    public int Quantity { get; init; }

    public QuoteItemProductData(ProductId productId, Money productPrice, int quantity)
    {
        ProductId = productId;
        ProductPrice = productPrice;
        Quantity = quantity;
    }
}