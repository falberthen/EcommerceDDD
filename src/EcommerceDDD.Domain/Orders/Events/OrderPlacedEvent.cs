using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Orders.Events
{
    public class OrderPlacedEvent : DomainEvent
    {
        public CustomerId CustomerId { get; private set; }
        public OrderId OrderId { get; private set; }

        public OrderPlacedEvent(CustomerId customerId, OrderId orderId)
        {
            CustomerId = customerId;
            OrderId = orderId;
            AggregateId = OrderId.Value;
        }
    }
}
