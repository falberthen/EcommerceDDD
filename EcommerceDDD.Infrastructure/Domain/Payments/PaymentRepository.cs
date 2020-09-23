using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Products
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EcommerceDDDContext _context;

        public PaymentRepository(EcommerceDDDContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Add(Payment payment, CancellationToken cancellationToken = default)
        {
            await _context.Payments.AddAsync(payment, cancellationToken);            
        }

        public async Task<Payment> GetById(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments.
                Include(p => p.Order).
                Include(p => p.Customer).
                FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
        }

        public async Task<Payment> GetByOrderId(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments.
                Include(p => p.Order).
                FirstOrDefaultAsync(x => x.Order.Id == orderId, cancellationToken);
        }
    }
}
