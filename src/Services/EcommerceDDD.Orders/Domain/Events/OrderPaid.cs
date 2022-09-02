using EcommerceDDD.Core.Domain;

namespace EcommerceDDD.Orders.Domain.Events;

public record OrderPaid(
    OrderId OrderId,
    PaymentId PaymentId, 
    List<ProductId> OrderLineProducts, 
    Money TotalPaid) : IDomainEvent
{
    public static OrderPaid Create(
        OrderId orderId,
        PaymentId paymentId, 
        List<ProductId> orderLineProducts, 
        Money totalPaid)
    {
        return new OrderPaid(orderId, paymentId, orderLineProducts, totalPaid);
    }
}
