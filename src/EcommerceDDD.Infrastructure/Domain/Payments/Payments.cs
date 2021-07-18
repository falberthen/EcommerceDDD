using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers.Orders;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Products
{
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

        public async Task<Payment> GetById(PaymentId paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
        }

        public async Task<Payment> GetByOrderId(OrderId orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        }
    }
}
