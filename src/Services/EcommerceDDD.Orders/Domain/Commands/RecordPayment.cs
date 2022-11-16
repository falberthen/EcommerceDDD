using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Domain.Commands;

public record class RecordPayment : ICommand
{
    public OrderId OrderId { get; private set; }
    public PaymentId PaymentId { get; private set; }
    public Money TotalPaid { get; private set; }

    public static RecordPayment Create(
        OrderId orderId,
        PaymentId paymentId,
        Money totalPaid)
    {
        if (paymentId is null)
            throw new ArgumentNullException(nameof(paymentId));
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (totalPaid is null)
            throw new ArgumentNullException(nameof(totalPaid));

        return new RecordPayment(orderId, paymentId, totalPaid);
    }

    private RecordPayment(
        OrderId orderId,
        PaymentId paymentId,
        Money totalPaid)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TotalPaid = totalPaid;
    }
}