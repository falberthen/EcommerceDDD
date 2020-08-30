using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.CurrencyExchange;
using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public class Order : Entity
    {
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ChangedAt { get; private set; }
        public bool IsCancelled { get; private set; }
        public Money TotalPrice { get; private set; }
        public List<OrderLine> OrderLines { get; private set; } = new List<OrderLine>();        

        public Order(Basket basket, ICurrencyConverter converter)
        {
            CreatedAt = DateTime.Now;            
            Status = OrderStatus.Placed;
            BuildOrderLines(basket, converter);
            CalculateTotalPrice(basket.Currency);
        }

        public void Change(Basket basket, ICurrencyConverter converter)
        {
            ChangedAt = DateTime.UtcNow;
            RemoveOrderLine(basket);
            BuildOrderLines(basket, converter);
            CalculateTotalPrice(basket.Currency);
        }

        private void BuildOrderLines(Basket basket, ICurrencyConverter converter)
        {
            foreach (var product in basket.Products)
            {
                var existingOrderLine = OrderLines.FirstOrDefault(o => o.ProductId == product.ProductId);
                if(existingOrderLine == null)
                {
                    OrderLines.Add(new OrderLine(
                        Id,
                        product.ProductId,
                        product.Price,
                        product.Quantity,
                        basket.Currency,
                        converter)
                    );
                }
                else
                {
                    var basketProduct = basket.Products.Single(p => p.ProductId == product.ProductId);
                    existingOrderLine.ChangeQuantity(basketProduct.Quantity, product.Price, basket.Currency, converter);
                }
            }
        }

        private void RemoveOrderLine(Basket cart)
        {
            var orderLines = OrderLines.ToList();
            foreach (var orderline in orderLines)
            {
                var product = cart.Products.SingleOrDefault(x => x.ProductId == orderline.ProductId);
                if (product == null)
                    OrderLines.Remove(orderline);
            }
        }

        private void CalculateTotalPrice(string currency)
        {
            var total = OrderLines.Sum(x => x.ProductExchangePrice.Value);
            TotalPrice = Money.Of(total, currency);
        }

        // Empty constructor for EF
        private Order() { }
    }
}
