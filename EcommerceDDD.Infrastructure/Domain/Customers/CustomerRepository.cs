using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Customers.Orders;
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

        public async Task RegisterCustomer(Customer customer, CancellationToken cancellationToken = default)
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task<Customer> GetCustomerById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .Where(c => c.Id == id)
                .Include("Orders.OrderLines")
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Customer> GetCustomerByEmail(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers.Where(c => c.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public void UpdateCustomer(Customer customer)
        {
            _dbContext.Customers.Update(customer);            
        }

        public async Task AddCustomerOrders(Customer customer)
        {
            if (customer.Orders.Count > 0)
            {
                var storedCustomer = await GetNoTrackedCustomerById(customer.Id);
                foreach (var order in customer.Orders)
                {
                    var found = storedCustomer.Orders.Find(ol => ol.Id == order.Id);
                    if (found == null)
                    {
                        _dbContext.Orders.Add(order);
                        _dbContext.OrderLines.AddRange(order.OrderLines);
                    }                        
                }
            }
        }

        public async Task ChangeCustomerOrder(Customer customer, Guid orderId)
        {
            var customerOrder = customer.Orders.Single(o => o.Id == orderId);
            if (customerOrder != null)
            {
                await SaveOrderlines(customer, orderId);
                _dbContext.Orders.Update(customerOrder);
            }
        }

        private async Task SaveOrderlines(Customer customer, Guid orderId)
        {
            var storedCustomer = await GetNoTrackedCustomerById(customer.Id);
            var storedCustomerOrder = storedCustomer.Orders.Single(o => o.Id == orderId);
            var curtomerOrderLines = customer.Orders.SelectMany(order => order.OrderLines).Where(o=>o.OrderId == orderId).ToList();

            foreach (var orderLine in curtomerOrderLines)
            {
                var found = storedCustomerOrder.OrderLines.Find(ol => ol.Id == orderLine.Id);
                if (found == null)
                    _dbContext.OrderLines.Add(orderLine);
                else
                    _dbContext.OrderLines.Update(orderLine);
            }
        }

        private async Task<Customer> GetNoTrackedCustomerById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers.AsNoTracking()
                .Where(c => c.Id == id)
                .Include("Orders.OrderLines")
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
