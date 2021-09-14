using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Products;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Quotes;

namespace EcommerceDDD.Domain
{
    public interface IEcommerceUnitOfWork : IUnitOfWork
    {
        ICustomers Customers { get; }
        IOrders Orders { get; }
        IProducts Products { get; }
        IQuotes Quotes { get; }
        IPayments Payments { get; }
        IStoredEvents StoredEvents { get; }
    }
}
