using System;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Customers.Orders.Events
{
    public class OrderChangedEvent : Event
    {
        public Guid OrderId { get; private set; }

        public OrderChangedEvent(Guid orderId)
        {
            OrderId = orderId; 
            AggregateId = orderId;
        }
    }
}
