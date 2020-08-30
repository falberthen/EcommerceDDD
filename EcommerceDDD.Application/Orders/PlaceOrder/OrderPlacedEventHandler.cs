using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers.Orders.Events;
using EcommerceDDD.Domain.Payments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
    {
        private IEcommerceUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderPlacedEventHandler(IEcommerceUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
        {
            _unitOfWork = unitOfWork;
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(OrderPlacedEvent orderPlacedEvent, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IEcommerceUnitOfWork>();

                //Creating a payment                
                var payment = new Payment(orderPlacedEvent.OrderId);
                await _unitOfWork.PaymentRepository.AddPayment(payment);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
