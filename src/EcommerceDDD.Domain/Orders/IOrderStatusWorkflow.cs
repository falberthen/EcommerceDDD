using EcommerceDDD.Domain.Payments;

namespace EcommerceDDD.Domain.Orders
{
    public interface IOrderStatusWorkflow
    {
        void CalculateOrderStatus(Order order, Payment payment);
    }
}
