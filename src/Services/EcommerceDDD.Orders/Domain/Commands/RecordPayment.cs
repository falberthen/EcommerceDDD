using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Domain.Commands;

public record class RecordPayment : ICommand
{
    public PaymentId PaymentId { get; private set; }
    public OrderId OrderId { get; private set; }
    public Money TotalPaid { get; private set; }

    public static RecordPayment Create(
        PaymentId paymentId,
        OrderId orderId,
        Money totalPaid)
    {
        if (paymentId is null)
            throw new ArgumentNullException(nameof(paymentId));
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (totalPaid is null)
            throw new ArgumentNullException(nameof(totalPaid));

        return new RecordPayment(paymentId, orderId, totalPaid);
    }

    private RecordPayment(
        PaymentId paymentId, 
        OrderId orderId, 
        Money totalPaid)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TotalPaid = totalPaid;
    }
}