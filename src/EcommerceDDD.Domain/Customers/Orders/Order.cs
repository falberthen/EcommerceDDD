using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public class Order : Entity<OrderId>
    {
        public CustomerId CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Money TotalPrice { get; private set; }
        public IReadOnlyList<OrderLine> OrderLines => _orderLines;
        private readonly List<OrderLine> _orderLines = new List<OrderLine>();

        private Order(CustomerId customerId, OrderId orderId, List<CartItemProductData> products, 
            Currency currency, ICurrencyConverter converter)
        {
            Id = orderId;
            CustomerId = customerId;
            CreatedAt = DateTime.Now;            
            Status = OrderStatus.Placed;
            BuildOrderLines(products, currency, converter);
        }

        internal static Order CreateNew(CustomerId customerId, List<CartItemProductData> products, 
            Currency currency, ICurrencyConverter converter)
        {
            return new Order(customerId, OrderId.Of(Guid.NewGuid()), products, currency, converter);
        }

        public void ChangeStatus(OrderStatus status)
        {
            Status = status;
        }

        private void BuildOrderLines(List<CartItemProductData> products, 
            Currency currency, ICurrencyConverter converter)
        {
            var orderLines = products.Select(c =>
                new OrderLine(
                    Guid.NewGuid(),
                    Id, 
                    c.ProductId,
                    c.ProductPrice,
                    c.Quantity,
                    currency,
                    converter)).ToArray();

            _orderLines.AddRange(orderLines);
            CalculateTotalPrice(currency);
        }

        private void CalculateTotalPrice(Currency currency)
        {
            var total = _orderLines.Sum(x => x.ProductExchangePrice.Value);
            TotalPrice = Money.Of(total, currency.Code);
        }

        // Empty constructor for EF
        private Order() { }
    }
}
