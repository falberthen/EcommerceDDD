using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Payments.Events;

namespace EcommerceDDD.Domain.Payments;

public class Payment : AggregateRoot<PaymentId>
{
    public OrderId OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public PaymentStatus Status { get; private set; }

    public static Payment CreateNew(CustomerId customerId, OrderId orderId)
    {
        if (orderId.Value == Guid.Empty)
            throw new BusinessRuleException("The order is required.");

        return new Payment(PaymentId.Of(Guid.NewGuid()), customerId, orderId);
    }

    public void MarkAsPaid()
    {
        PaidAt = DateTime.Now;
        Status = PaymentStatus.Paid;
        AddDomainEvent(new PaymentAuthorizedEvent(Id));
    }

    private Payment(PaymentId id, CustomerId customerId, OrderId orderId)
    {
        if (orderId.Value == Guid.Empty)
            throw new BusinessRuleException("The order is required.");

        Id = id;
        OrderId = orderId;
        CustomerId = customerId;
        CreatedAt = DateTime.Now;
        Status = PaymentStatus.ToPay;
        AddDomainEvent(new PaymentCreatedEvent(Id));
    }

    // Empty constructor for EF
    private Payment() { }
}