using System;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Domain.Orders.Events
{
    public class OrderPlacedEvent : Event
    {
        public Guid OrderId { get; private set; }

        public OrderPlacedEvent(Guid orderId)
        {
            OrderId = orderId;
            AggregateId = OrderId;
        }
    }
}
