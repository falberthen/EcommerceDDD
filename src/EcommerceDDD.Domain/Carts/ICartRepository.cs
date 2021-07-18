using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Carts
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task AddCart(Cart cart, CancellationToken cancellationToken = default);
        Task<Cart> GetCartById(CartId cartId, CancellationToken cancellationToken = default);
        Task<Cart> GetCartByCustomerId(CustomerId customerId, CancellationToken cancellationToken = default);
    }
}
