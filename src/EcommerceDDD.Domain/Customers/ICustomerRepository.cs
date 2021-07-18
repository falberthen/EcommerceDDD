using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.SeedWork;

namespace EcommerceDDD.Domain.Customers
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task AddCustomer(Customer customer, CancellationToken cancellationToken = default);
        Task<Customer> GetCustomerById(CustomerId id, CancellationToken cancellationToken = default);
        Task<Customer> GetCustomerByEmail(string email, CancellationToken cancellationToken = default);
    }
}
