using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Core.Base;

namespace EcommerceDDD.Domain.Carts
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task Add(Cart cart, CancellationToken cancellationToken = default);
        Task<Cart> GetById(Guid cartId, CancellationToken cancellationToken = default);
        Task<Cart> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    }
}
