using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDDDContext _dbContext;

        public OrderRepository(EcommerceDDDContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task Add(Order order, CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order);
        }

        public async Task<Order> GetById(Guid orderId, CancellationToken cancellationToken = default)
        {            
            return await _dbContext.Orders
                .Include(o => o.OrderLines)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(c => c.Id == orderId, cancellationToken);
        }

        public async Task<List<Order>> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderLines)
                .Include(o => o.Customer)
                .Where(o => o.Customer.Id == customerId)
                .ToListAsync(cancellationToken);
        }
    }
}