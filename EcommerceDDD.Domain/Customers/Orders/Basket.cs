using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public class Basket
    {
        public string Currency { get; private set; }
        public List<BasketProduct> Products { get; private set; } = new List<BasketProduct>();

        public Basket(string currency)
        {
            if (string.IsNullOrEmpty(currency))
                throw new BusinessRuleException("The used currency must be informed.");

            Currency = currency;
        }

        public void AddProduct(Guid productId, Money price, int quantity)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));

            if (price == null)
                throw new ArgumentNullException(nameof(price));
      
            Products.Add(new BasketProduct(productId, price, quantity));
        }

        public class BasketProduct
        {
            public Guid ProductId { get; private set; }
            public Money Price { get; private set; }
            public int Quantity { get; private set; }

            public BasketProduct(Guid productId, Money price, int quantity)
            {
                ProductId = productId;
                Price = price;
                Quantity = quantity;
            }
        }
    }
}
