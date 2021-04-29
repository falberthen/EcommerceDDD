using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Carts
{
    public class CartItemProductData
    {
        public ProductId ProductId { get; }
        public Money ProductPrice { get; }
        public int Quantity { get; }

        public CartItemProductData(ProductId productId, Money productPrice, int quantity)
        {
            ProductId = productId;
            ProductPrice = productPrice;
            Quantity = quantity;
        }
    }
}
