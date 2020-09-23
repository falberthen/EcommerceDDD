using System;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Customers.Events
{
    public class CustomerRegisteredEvent : Event
    {
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }

        public CustomerRegisteredEvent(Guid customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
            AggregateId = CustomerId;
        }
    }
}
