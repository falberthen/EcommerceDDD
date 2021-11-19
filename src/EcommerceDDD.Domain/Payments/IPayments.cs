using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Payments;

public interface IPayments : IRepository<Payment>
{
    Task Add(Payment payment, CancellationToken cancellationToken = default);
    Task<Payment> GetById(PaymentId paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> Find(Specification<Payment> specification, CancellationToken cancellationToken = default);
}