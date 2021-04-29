using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Customers.Events;
using MediatR;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class CustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        public CustomerRegisteredEventHandler()
        {
        }

        public async Task Handle(CustomerRegisteredEvent customerRegisteredEvent, CancellationToken cancellationToken)
        {
            // Here we could send an email to customer informing the user was registred with success
            await Task.CompletedTask;
        }
    }
}
