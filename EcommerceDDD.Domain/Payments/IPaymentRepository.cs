using EcommerceDDD.Domain.Core.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Payments
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task Add(Payment payment, CancellationToken cancellationToken = default);
        Task<Payment> GetById(Guid paymentId, CancellationToken cancellationToken = default);
        Task<Payment> GetByOrderId(Guid orderId, CancellationToken cancellationToken = default);
    }
}
