using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Payments;
using EcommerceDDD.Domain.Customers.Events;
using System;
using System.Linq;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public OrderPlacedEventHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderPlacedEvent orderPlacedEvent, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(orderPlacedEvent.CustomerId, cancellationToken);

            var order = customer.Orders.
                Where(o => o.Id == orderPlacedEvent.OrderId)
                .FirstOrDefault();

            if (order == null)
                throw new InvalidDataException("Order not found.");

            // Creating a payment
            var payment = new Payment(PaymentId.Of(Guid.NewGuid()), order.CustomerId, order.Id);
            await _unitOfWork.PaymentRepository.AddPayment(payment);
            await _unitOfWork.CommitAsync();            
        }
    }
}
