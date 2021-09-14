using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Orders
{
    public class Orders : IOrders
    {
        private readonly EcommerceDDDContext _dbContext;

        public Orders(EcommerceDDDContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task Add(Order order, CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> Find(Specification<Order> specification, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Where(specification.ToExpression())
                .ToListAsync();
        }

        public async Task<Order> GetById(OrderId orderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Where(c => c.Id == orderId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}