using System;
using System.Collections.Generic;
using System.Text;
using EcommerceDDD.Domain.Payments.Events;
using EcommerceDDD.Domain.Core.Base;

namespace EcommerceDDD.Domain.Payments
{
    public class Payment : Entity, IAggregateRoot
    {
        public Guid OrderId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public PaymentStatus Status { get; private set; }
        public bool ConfirmationEmailSent { get; private set; }

        public Payment(Guid orderId)
        {
            OrderId = orderId;
            CreatedAt = DateTime.Now;
            Status = PaymentStatus.ToPay;
            AddDomainEvent(new PaymentCreatedEvent(Id, OrderId));
        }

        public void PaymentMade()
        {
            PaidAt = DateTime.Now;
            //TODO: Add event
        }

        public void SetPaymentStatus()
        {
            //TODO: Will be changed when paid
        }

        public void SetConfirmationEmailSent()
        {
            ConfirmationEmailSent = true;
        }

        // Empty constructor for EF
        private Payment() { }
    }
}
