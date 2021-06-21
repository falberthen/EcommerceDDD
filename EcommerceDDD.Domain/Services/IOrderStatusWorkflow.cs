using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.Payments;

namespace EcommerceDDD.Domain.Services
{
    public interface IOrderStatusWorkflow
    {
        void CalculateOrderStatus(Order order, Payment payment);
    }
}
