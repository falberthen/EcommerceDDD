using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Payments.Events;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PaymentAuthorizedEventHandler : INotificationHandler<PaymentAuthorizedEvent>
    {
        public async Task Handle(PaymentAuthorizedEvent paymentAuthorizedEvent, CancellationToken cancellationToken)
        {
            // Send an email to customer informing the order was placed with success
            await Task.CompletedTask;
        }
    }
}
