using System.Linq;
using System.Collections.Generic;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Infrastructure.Database.Context;

namespace EcommerceDDD.Infrastructure.Domain.Payments;

public class Payments : IPayments
{
    private readonly EcommerceDDDContext _context;

    public Payments(EcommerceDDDContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Add(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> Find(Specification<Payment> specification, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(specification.ToExpression())
            .ToListAsync();
    }

    public async Task<Payment> GetById(PaymentId paymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
    }
}