using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers.Orders;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Payments
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task AddPayment(Payment payment, CancellationToken cancellationToken = default);
        Task<Payment> GetPaymentById(PaymentId paymentId, CancellationToken cancellationToken = default);
        Task<Payment> GetPaymentByOrderId(OrderId orderId, CancellationToken cancellationToken = default);
    }
}
