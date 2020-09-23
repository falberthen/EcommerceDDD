using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders.Events;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain.Shared;

namespace EcommerceDDD.Domain.Orders
{
    public class Order : AggregateRoot<Guid>
    {
        public Customer Customer { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Money TotalPrice { get; private set; }
        public IReadOnlyList<OrderLine> OrderLines => _orderLines;
        private readonly List<OrderLine> _orderLines = new List<OrderLine>();

        private Order(Guid id, Cart cart, Currency currency, ICurrencyConverter converter)
        {
            Id = id;
            CreatedAt = DateTime.Now;            
            Status = OrderStatus.Placed;
            Customer = cart.Customer;
            BuildOrderLines(cart, currency, converter);
            AddDomainEvent(new OrderPlacedEvent(Id));
        }

        public static Order PlaceOrder(Guid id, Cart cart, Currency currency, ICurrencyConverter currencyConverter)
        {
            if (!cart.Items.Any())
                throw new BusinessRuleException("An order should have at least one product.");

            if (currency == null)
                throw new BusinessRuleException("The currency is required.");

            var order = new Order(id, cart, currency, currencyConverter);
            return order;
        }

        public void ChangeStatus(OrderStatus status)
        {
            Status = status;
        }

        private void BuildOrderLines(Cart cart, Currency currency, ICurrencyConverter converter)
        {
            var orderLines = cart.Items.Select(c =>
                new OrderLine(
                    Guid.NewGuid(),
                    Id, 
                    c.Product.Id, 
                    c.Product.Price, 
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
