using EcommerceDDD.Domain.Core.Events;

namespace EcommerceDDD.Domain.Customers.Events
{
    public class CustomerUpdatedEvent : DomainEvent
    {
        public CustomerId CustomerId { get; private set; }
        public string Name { get; private set; }

        public CustomerUpdatedEvent(CustomerId customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
            AggregateId = CustomerId.Value;
        }        
    }
}
