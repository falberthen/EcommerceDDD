using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Carts
{
    public interface ICarts : IRepository<Cart>
    {
        Task Add(Cart cart, CancellationToken cancellationToken = default);
        Task<Cart> GetById(CartId cartId, CancellationToken cancellationToken = default);
        Task<Cart> GetByCustomerId(CustomerId customerId, CancellationToken cancellationToken = default);
    }
}
