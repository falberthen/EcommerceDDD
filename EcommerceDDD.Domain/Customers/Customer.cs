using System;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Customers.Events;
using EcommerceDDD.Domain.Services;

namespace EcommerceDDD.Domain.Customers
{
    public class Customer : AggregateRoot<Guid>
    {
        public string Email { get; private set; }
        public string Name { get; private set; }
        
        public static Customer CreateCustomer(Guid id, string email, string name,
            ICustomerUniquenessChecker customerUniquenessChecker)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be null or whitespace.", nameof(name));

            if (!customerUniquenessChecker.IsUserUnique(email))
                throw new BusinessRuleException("This e-mail is already in use.");

            return new Customer(id, email, name);
        }

        public void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Name = value;
            AddDomainEvent(new CustomerUpdatedEvent(Id, Name));
        }

        private Customer(Guid id, string email, string name)
        {
            Id = id;
            Email = email;
            Name = name;
            AddDomainEvent(new CustomerRegisteredEvent(Id, Name));
        }

        // Empty constructor for EF
        private Customer() { }
    }
}
