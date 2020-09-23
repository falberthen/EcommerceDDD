using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcommerceDDD.Domain.Carts
{
    public class CartItem : Entity
    {
        public Product Product { get; private set; }
        public int Quantity { get; private set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
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
