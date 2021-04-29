using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Orders.Events;
using EcommerceDDD.Domain.Payments;
using System;

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
            var order = await _unitOfWork.OrderRepository.GetById(orderPlacedEvent.OrderId);

            if (order == null)
                throw new InvalidDataException("Order not found.");

            // Creating a payment
            var payment = new Payment(Guid.NewGuid(), order);
            await _unitOfWork.PaymentRepository.Add(payment);
            await _unitOfWork.CommitAsync();            
        }
    }
}
