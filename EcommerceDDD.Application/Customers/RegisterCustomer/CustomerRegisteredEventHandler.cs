using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers.Events;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class CustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomerRegisteredEventHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(CustomerRegisteredEvent customerRegisteredEvent, CancellationToken cancellationToken)
        {
            // Send an email to customer informing the order was placed with success
            await Task.CompletedTask;
        }
    }
}
