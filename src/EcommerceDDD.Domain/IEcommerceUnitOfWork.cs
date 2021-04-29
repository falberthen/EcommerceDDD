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
        ICustomerRepository CustomerRepository { get; }
        IStoredEventRepository MessageRepository { get; }
        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }
        IPaymentRepository PaymentRepository { get; }
    }
}
