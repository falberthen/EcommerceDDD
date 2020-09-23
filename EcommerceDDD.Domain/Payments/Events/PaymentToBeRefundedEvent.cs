using EcommerceDDD.Domain.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Domain.Payments.Events
{
    public class PaymentToBeRefundedEvent : Event
    {
        public Guid PaymentId { get; private set; }
        public Guid OrderId { get; private set; }

        public PaymentToBeRefundedEvent(Guid paymentId, Guid orderId)
        {
            PaymentId = paymentId;
            OrderId = orderId;
            AggregateId = paymentId;
        }
    }
}
