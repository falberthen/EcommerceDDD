using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.SeedWork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Orders
{
    public interface IOrders
    {
        Task Add(Order order, CancellationToken cancellationToken = default);
        Task<Order> GetById(OrderId orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> Find(Specification<Order> specification, CancellationToken cancellationToken = default);
    }
}
