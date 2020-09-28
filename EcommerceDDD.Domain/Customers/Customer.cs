using System;
using System.Collections.Generic;
using System.Linq;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Customers.Events;
using EcommerceDDD.Domain.Services;

namespace EcommerceDDD.Domain.Customers
{
    public class Customer : Entity, IAggregateRoot
    {
        public string Email { get; private set; }
        public string Name { get; private set; }
        
        public static Customer CreateCustomer(string email, string name,
            ICustomerUniquenessChecker customerUniquenessChecker)
        {
            if (!customerUniquenessChecker.IsUserUnique(email))
                throw new BusinessRuleException("This e-mail is already in use.");

            return new Customer(email, name);
        }

        public void SetName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Name = value;
            AddDomainEvent(new CustomerUpdatedEvent(Id, Name));
        }

        private Customer(string email, string name)
        {
            Email = email;
            Name = name;
            AddDomainEvent(new CustomerRegisteredEvent(Id, Name));
        }

        // Empty constructor for EF
        private Customer() { }
    }
}
