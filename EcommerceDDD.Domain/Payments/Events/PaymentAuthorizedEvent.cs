using EcommerceDDD.Domain.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Domain.Payments.Events
{
    public class PaymentAuthorizedEvent : Event
    {
        public Guid PaymentId { get; private set; }

        public PaymentAuthorizedEvent(Guid paymentId)
        {
            PaymentId = paymentId;
            AggregateId = paymentId;
        }
    }
}
