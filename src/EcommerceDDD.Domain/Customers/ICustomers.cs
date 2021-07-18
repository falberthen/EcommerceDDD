using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Customers
{
    public interface ICustomers : IRepository<Customer>
    {
        Task Add(Customer customer, CancellationToken cancellationToken = default);
        Task<Customer> GetById(CustomerId id, CancellationToken cancellationToken = default);
        Task<Customer> GetByEmail(string email, CancellationToken cancellationToken = default);
    }
}
