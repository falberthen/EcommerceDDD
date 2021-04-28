using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderPlacedEventHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(OrderPlacedEvent orderPlacedEvent, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IEcommerceUnitOfWork>();
                var order = await unitOfWork.OrderRepository.GetById(orderPlacedEvent.OrderId);

                if (order == null)
                    throw new InvalidDataException("Order not found.");

                // Creating a payment
                var payment = new Payment(Guid.NewGuid(), order);
                await unitOfWork.PaymentRepository.Add(payment);
                await unitOfWork.CommitAsync();
            }
        }
    }
}
