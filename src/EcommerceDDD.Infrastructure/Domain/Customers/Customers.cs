using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDDD.Infrastructure.Domain.Customers
{
    public class Customers : ICustomers
    {
        private readonly EcommerceDDDContext _dbContext;

        public Customers(EcommerceDDDContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task Add(Customer customer, CancellationToken cancellationToken = default)
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task<Customer> GetById(CustomerId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Customer> GetByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
