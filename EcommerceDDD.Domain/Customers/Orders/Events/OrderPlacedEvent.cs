using System;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Customers.Orders.Events
{
    public class OrderPlacedEvent : Event
    {
        public Guid CustomerId { get; private set; }
        public Guid OrderId { get; private set; }

        public OrderPlacedEvent(
            Guid customerId,
            Guid orderId)
        {
            CustomerId = customerId; 
            OrderId = orderId; 
            AggregateId = OrderId;
        }
    }
}
