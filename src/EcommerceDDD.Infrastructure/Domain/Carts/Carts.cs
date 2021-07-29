using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Carts
{
    public class Carts : ICarts
    {
        private readonly EcommerceDDDContext _context;

        public Carts(EcommerceDDDContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Add(Cart cart, CancellationToken cancellationToken = default)
        {
            await _context.Carts.AddAsync(cart, cancellationToken);
        }

        public async Task<Cart> GetById(CartId cartId, CancellationToken cancellationToken = default)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);
        }

        public async Task<Cart> GetByCustomerId(CustomerId customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        }
    }
}
