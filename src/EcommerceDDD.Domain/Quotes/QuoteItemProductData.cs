using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Quotes
{
    public class QuoteItemProductData
    {
        public ProductId ProductId { get; }
        public Money ProductPrice { get; }
        public int Quantity { get; }

        public QuoteItemProductData(ProductId productId, Money productPrice, int quantity)
        {
            ProductId = productId;
            ProductPrice = productPrice;
            Quantity = quantity;
        }
    }
}
