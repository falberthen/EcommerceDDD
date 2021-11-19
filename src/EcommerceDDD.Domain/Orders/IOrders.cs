using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Orders;

public interface IOrders
{
    Task Add(Order order, CancellationToken cancellationToken = default);
    Task<Order> GetById(OrderId orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> Find(Specification<Order> specification, CancellationToken cancellationToken = default);
}