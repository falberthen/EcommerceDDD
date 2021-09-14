using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Payments;
using System.Linq;
using EcommerceDDD.Domain.Orders.Events;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public OrderPlacedEventHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderPlacedEvent orderPlacedEvent, 
            CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders
                .GetById(orderPlacedEvent.OrderId, cancellationToken);

            if (order == null)
                throw new InvalidDataException("Order not found.");

            // Creating a payment
            var payment = Payment
                .CreateNew(order.CustomerId, order.Id);

            await _unitOfWork.Payments
                .Add(payment);

            await _unitOfWork.CommitAsync();            
        }
    }
}
