using System;
using EcommerceDDD.Domain.Payments.Events;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Payments
{
    public class Payment : AggregateRoot<Guid>
    {
        public Order Order { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public PaymentStatus Status { get; private set; }

        public Payment(Guid id, Order order)
        {
            if (order == null)
                throw new BusinessRuleException("The order is required.");

            Id = id;
            Order = order;
            Customer = order.Customer;
            CreatedAt = DateTime.Now;
            Status = PaymentStatus.ToPay;
            AddDomainEvent(new PaymentCreatedEvent(Id));
        }

        public void MarkAsPaid()
        {
            PaidAt = DateTime.Now;
            Status = PaymentStatus.Paid;
            AddDomainEvent(new PaymentAuthorizedEvent(Id));
        }

        // Empty constructor for EF
        private Payment() { }
    }
}
