using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers.Orders;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Payments
{
    public interface IPayments : IRepository<Payment>
    {
        Task Add(Payment payment, CancellationToken cancellationToken = default);
        Task<Payment> GetById(PaymentId paymentId, CancellationToken cancellationToken = default);
        Task<Payment> GetByOrderId(OrderId orderId, CancellationToken cancellationToken = default);
    }
}
