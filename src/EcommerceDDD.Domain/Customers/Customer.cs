using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers.Events;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.SharedKernel;

namespace EcommerceDDD.Domain.Customers
{
    public class Customer : AggregateRoot<CustomerId>
    {
        public string Email { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<Order> Orders => _orders;
        private readonly List<Order> _orders = new List<Order>();        
        public Cart Cart { get; }

        public static Customer CreateCustomer(string email, string name,
            ICustomerUniquenessChecker customerUniquenessChecker)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be null or whitespace.", nameof(name));

            if (!customerUniquenessChecker.IsUserUnique(email))
                throw new BusinessRuleException("This e-mail is already in use.");

            var customerId = new CustomerId(Guid.NewGuid());
            return new Customer(customerId, email, name);
        }

        public Order PlaceOrder(CustomerId customerId, List<CartItemProductData> products, 
            Currency currency, ICurrencyConverter currencyConverter)
        {
            if (customerId == null)
                throw new BusinessRuleException("The customer Id is required.");

            if (!products.Any())
                throw new BusinessRuleException("An order should have at least one product.");

            if (currency == null)
                throw new BusinessRuleException("The currency is required.");

            var order = Order.CreateNew(customerId, products, currency, currencyConverter);
            _orders.Add(order);

            AddDomainEvent(new OrderPlacedEvent(customerId, order.Id));

            return order;
        }

        public void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Name = value;
            AddDomainEvent(new CustomerUpdatedEvent(Id, Name));
        }

        private Customer(CustomerId id, string email, string name)
        {
            Id = id;
            Email = email;
            Name = name;
            AddDomainEvent(new CustomerRegisteredEvent(id, Name));
        }

        // Empty constructor for EF
        private Customer() { }
    }
}
