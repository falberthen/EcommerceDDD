using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceDDDContext _dbContext;

        public CustomerRepository(EcommerceDDDContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddCustomer(Customer customer, CancellationToken cancellationToken = default)
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task<Customer> GetCustomerById(CustomerId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Customer> GetCustomerByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
