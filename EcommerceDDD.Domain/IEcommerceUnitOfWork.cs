using EcommerceDDD.Domain.Carts;
using EcommerceDDD.Domain.Core.Base;
using EcommerceDDD.Domain.Core.Messaging;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Products;

namespace EcommerceDDD.Domain
{
    public interface IEcommerceUnitOfWork : IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IStoredEventRepository MessageRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICartRepository CartRepository { get; }
        IPaymentRepository PaymentRepository { get; }
    }
}
