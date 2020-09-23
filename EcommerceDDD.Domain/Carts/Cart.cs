using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Products;

namespace EcommerceDDD.Domain.Carts
{
    public class Cart : AggregateRoot<Guid>
    {
        public Customer Customer { get; private set; }
        public IReadOnlyCollection<CartItem> Items => _items;        
        private readonly List<CartItem> _items = new List<CartItem>();

        public Cart(Guid id, Customer customer)
        {
            Id = id;
            if (customer == null)
                throw new BusinessRuleException("The customer is required.");

            Customer = customer;
        }

        public CartItem AddItem(Product product, int quantity)
        {
            if (product == null)
                throw new BusinessRuleException("The cart item must have a product.");

            if (quantity == 0)
                throw new BusinessRuleException("The product quantity must be at last 1.");

            var cartItem = new CartItem(Guid.NewGuid(), product, quantity);
            _items.Add(cartItem);

            return cartItem;
        }

        public CartItem ChangeCart(Product product, int quantity)
        {
            if (product == null)
                throw new BusinessRuleException("The cart item must have a product.");

            var cartItem = _items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (cartItem == null) 
                cartItem = AddItem(product, quantity); // Add item
            else if(quantity == 0) 
                RemoveItem(cartItem.Id); // Remove Item
            else 
                cartItem.ChangeQuantity(quantity); // Change item quantity

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
