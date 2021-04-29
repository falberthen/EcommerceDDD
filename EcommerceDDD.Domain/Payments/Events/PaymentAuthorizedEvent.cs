using EcommerceDDD.Domain.Core.Messaging;
using System;

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
