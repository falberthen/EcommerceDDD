using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.SeedWork;
using EcommerceDDD.Domain.Core.Events;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Products;

namespace EcommerceDDD.Domain
{
    public interface IEcommerceUnitOfWork : IUnitOfWork
    {
        ICustomers Customers { get; }
        IProducts Products { get; }
        ICarts Carts { get; }
        IPayments Payments { get; }
        IStoredEvents StoredEvents { get; }
    }
}
