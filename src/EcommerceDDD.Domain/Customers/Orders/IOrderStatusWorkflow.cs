using EcommerceDDD.Domain.Payments;

namespace EcommerceDDD.Domain.Customers.Orders
{
    public interface IOrderStatusWorkflow
    {
        void CalculateOrderStatus(Order order, Payment payment);
    }
}
