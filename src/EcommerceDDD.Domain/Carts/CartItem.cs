using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Products;
using System;

namespace EcommerceDDD.Domain.Carts
{
    public class CartItem : Entity<Guid>
    {
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }

        public CartItem(Guid id, ProductId productId, int quantity)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
        }

        public void ChangeQuantity(int quantity)
        {
            if (quantity == 0)
                throw new BusinessRuleException("The product quantity must be at last 1.");

            Quantity = quantity;
        }

        // Empty constructor for EF
        private CartItem() { }
    }
}
