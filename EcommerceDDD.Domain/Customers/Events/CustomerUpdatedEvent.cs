using System;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Customers.Events
{
    public class CustomerUpdatedEvent : Event
    {
        public CustomerUpdatedEvent(Guid customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
            AggregateId = CustomerId;
        }
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }
    }
}
