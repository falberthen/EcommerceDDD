using EcommerceDDD.Domain.Core.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task Add(Order order, CancellationToken cancellationToken = default);
        Task<Order> GetById(Guid orderId, CancellationToken cancellationToken = default);
        Task<List<Order>> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    }
}
