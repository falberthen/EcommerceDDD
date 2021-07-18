using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Carts
{
    public class Cart : AggregateRoot<CartId>
    {        
        public CustomerId CustomerId { get; private set; }
        public IReadOnlyCollection<CartItem> Items => _items;        
        private readonly List<CartItem> _items = new List<CartItem>();

        public Cart(CartId cartId, CustomerId customerId)
        {
            Id = cartId;
            if (customerId == null)
                throw new BusinessRuleException("The customer is required.");

            CustomerId = customerId;
        }

        public CartItem AddItem(CartItemProductData productData)
        {
            if (productData == null)
                throw new BusinessRuleException("The cart item must have a product.");

            if (productData.Quantity == 0)
                throw new BusinessRuleException("The product quantity must be at last 1.");

            var cartItem = new CartItem(Guid.NewGuid(), 
                productData.ProductId, 
                productData.Quantity);

            _items.Add(cartItem);

            return cartItem;
        }

        public CartItem ChangeCart(CartItemProductData productData)
        {
            if (productData == null)
                throw new BusinessRuleException("The cart item must have a product.");

            var cartItem = _items.FirstOrDefault((Func<CartItem, bool>)(i => i.ProductId.Value == productData.ProductId.Value));

            if (cartItem == null) 
                cartItem = AddItem(productData); // Add item
            else if(productData.Quantity == 0) 
                RemoveItem(cartItem.Id); // Remove Item
            else 
                cartItem.ChangeQuantity(productData.Quantity); // Change item quantity

            return cartItem;
        }

        public void RemoveItem(Guid cartItemId)
        {
            var cartItem = _items.FirstOrDefault(i => i.Id == cartItemId);
            if (cartItem == null)
                throw new BusinessRuleException("Invalid cart item.");

            _items.Remove(cartItem);
        }

        public void Clear()
        {            
            _items.Clear();            
        }

        // Empty constructor for EF
        private Cart() { }
    }
}
